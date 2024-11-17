using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using CityNexus.Modulith.Application.Modules.Contracts.Abstractions.ClickSign;
using CityNexus.Modulith.Application.Modules.Contracts.Extensions;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using CityNexus.Modulith.Infra.Options;
using CityNexus.Modulith.Infra.Persistence.Dapper;
using CityNexus.Modulith.Infra.Persistence.EF;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        AddPersistence(services, configuration);
        services.AddContractModule();
        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Default")
            ?? throw new NullReferenceException("Default connection string is null");
        SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
            connectionString
        ));
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
}
