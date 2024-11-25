using CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;
using Microsoft.Extensions.DependencyInjection;

namespace CityNexus.Modulith.Contracts.Contracts.Extensions;

public static class ContractModuleExtensions
{
    public static IServiceCollection AddContractModule(this IServiceCollection services)
    {
        services.AddScoped<CreateAgreementContractCommandHandler>();
        return services;
    }
}
