using CityNexus.Modulith.Application.Modules.Shared.Abstractions;

namespace CityNexus.Modulith.Infra.Persistence.EF;

public sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
