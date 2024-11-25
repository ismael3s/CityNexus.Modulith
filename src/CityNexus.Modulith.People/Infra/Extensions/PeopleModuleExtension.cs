using CityNexus.Modulith.People.Application.Commands.RegisterPerson;
using CityNexus.Modulith.People.Application.Queries.FindPeople;
using CityNexus.Modulith.People.Application.Repositories;
using CityNexus.Modulith.People.Infra.Persistence.Dapper;
using CityNexus.Modulith.People.Infra.Persistence.EF;
using CityNexus.Modulith.People.Infra.Persistence.EF.Repositories;
using CityNexus.Modulith.SharedKernel.Abstractions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.Modulith.People.Infra.Extensions;

public static class PeopleModuleExtension
{
    public static IServiceCollection AddPeopleModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Default")
            ?? throw new NullReferenceException("Default connection string is null");
        SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddDbContext<PersonDbContext>(o => o.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "people");
        }));
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
            connectionString
        ));
        return services
                .AddScoped<RegisterPersonCommandHandler>()
                .AddScoped<FindPeopleQueryHandler>()
                .AddScoped<IPersonRepository, EfPersonRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
            ;
    }
}
