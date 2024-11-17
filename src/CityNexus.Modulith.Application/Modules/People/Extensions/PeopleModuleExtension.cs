using CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.Modulith.Application.Modules.People.Extensions;

public static class PeopleModuleExtension
{
    public static IServiceCollection AddPeopleModule(this IServiceCollection services)
    {
        return services.AddScoped<RegisterPersonCommandHandler>();
    }
}
