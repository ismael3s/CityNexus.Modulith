using CityNexus.Modulith.SharedKernel.Abstractions;

namespace CityNexus.Modulith.People.Infra.Persistence.EF;

public sealed class UnitOfWork(PersonDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
