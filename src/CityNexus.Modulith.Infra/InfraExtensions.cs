using System.Text.Json;
using Asp.Versioning;
using CityNexus.Modulith.Contracts.Contracts.Abstractions.ClickSign;
using CityNexus.Modulith.Contracts.Contracts.Extensions;
using CityNexus.Modulith.Infra.Options;
using CityNexus.Modulith.Infra.Persistence.OutboxMessage;
using CityNexus.Modulith.People.Domain.Entities.People.Events;
using CityNexus.Modulith.People.Infra.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RabbitMQ.Client;
using Refit;

namespace CityNexus.Modulith.Infra;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ConfigureOptions(services, configuration);
        AddApiVersioning(services);
        AddApplicationHealthChecks(services);
        AddCorsPolicy(services);
        AddRefitClients(services, configuration);
        // AddPersistence(services, configuration);
        AddBackgroundJobs(services, configuration);
        AddMassTransit(services, configuration);
        
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PeopleModuleExtension).Assembly))
            .AddContractModule().AddPeopleModule(configuration)
            ;
    }

    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQuartz()
            .AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true)
            .ConfigureOptions<ProcessOutboxJobSetup>();
    }
    
    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ClickSignOption>()
            .Bind(configuration.GetSection("ClickSign"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services
            .AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("ver")
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    private static void AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    private static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(x =>
        {
            x.AddDefaultPolicy(opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
    }

    private static void AddRefitClients(IServiceCollection services, IConfiguration configuration)
    {
        var clickSignClientConfiguration = configuration
            .GetSection("ClickSign")
            .Get<ClickSignOption>();
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
        services
            .AddRefitClient<IClickSignGateway>(
                new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(options),
                }
            )
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(clickSignClientConfiguration!.ApiUrl);
                c.DefaultRequestHeaders.Add("Authorization", clickSignClientConfiguration.ApiKey);
            })
            .AddStandardResilienceHandler();
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitmqCfg = configuration.GetSection("RabbitMq").Get<RabbitMqOption>();
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderConsumer>();
            x.AddConfigureEndpointsCallback(
                (context, name, cfg) =>
                {
                    cfg.UseMessageRetry(r =>
                        r.Exponential(
                            5,
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromMinutes(2),
                            TimeSpan.FromSeconds(10)
                        )
                    );
                }
            );
            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        rabbitmqCfg!.Host,
                        rabbitmqCfg.Port,
                        rabbitmqCfg.VHost,
                        hostCfg =>
                        {
                            hostCfg.Username(rabbitmqCfg.Username);
                            hostCfg.Password(rabbitmqCfg.Password);
                        }
                    );

                    cfg.Message<RegisteredPersonDomainEvent>(cfg =>
                    {
                        cfg.SetEntityName("person.registered");
                    });
                    MessageCorrelation.UseCorrelationId<RegisteredPersonDomainEvent>(@event =>
                        @event.Id
                    );
                    cfg.Publish<RegisteredPersonDomainEvent>(pCfg =>
                    {
                        cfg.ExchangeType = ExchangeType.Fanout;
                        pCfg.Durable = true;

                        pCfg.BindQueue(
                            "person.registered",
                            "person.registered.notifications",
                            que =>
                            {
                                que.SetQueueArgument(
                                    "x-dead-letter-exchange",
                                    "dlq.person.registered"
                                );
                                que.SetQueueArgument(
                                    "x-dead-letter-routing-key",
                                    "dlq.person.registered.notifications"
                                );
                            }
                        );

                        // pCfg.BindQueue("dlq.person.registered", "dlq.person.registered.notifications");
                        // pCfg.BindQueue("dlq.person.registered", "dlq.person.registered.contracts");
                        pCfg.BindQueue(
                            "person.registered",
                            "person.registered.contracts",
                            qCfg =>
                            {
                                qCfg.SetQueueArgument(
                                    "x-dead-letter-exchange",
                                    "dlq.person.registered"
                                );
                                qCfg.SetQueueArgument(
                                    "x-dead-letter-routing-key",
                                    "dlq.person.registered.contracts"
                                );
                            }
                        );
                    });

                    cfg.ReceiveEndpoint(
                        "person.registered.contracts",
                        e =>
                        {
                            e.ConfigureConsumer<OrderConsumer>(context);
                            // e.Consumer<OrderConsumer>();
                            e.Bind("person.registered", x => { });
                            e.SetQueueArgument("x-dead-letter-exchange", "dlq.person.registered");
                            e.SetQueueArgument(
                                "x-dead-letter-routing-key",
                                "dlq.person.registered.contracts"
                            );
                        }
                    );
                    cfg.ConfigureEndpoints(context);
                }
            );
        });
    }
}
