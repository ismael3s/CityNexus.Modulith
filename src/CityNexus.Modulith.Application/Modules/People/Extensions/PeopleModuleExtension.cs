using CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;
using CityNexus.Modulith.Application.Modules.People.Queries.FindPeople;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.Modulith.Application.Modules.People.Extensions;

public static class PeopleModuleExtension
{
    public static IServiceCollection AddPeopleModule(this IServiceCollection services)
    {
        return services.AddScoped<RegisterPersonCommandHandler>()
                .AddScoped<FindPeopleQueryHandler>()
            ;
    }
}
