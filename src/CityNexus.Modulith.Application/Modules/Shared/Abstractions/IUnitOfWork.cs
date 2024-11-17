namespace CityNexus.Modulith.Application.Modules.Shared.Abstractions;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}
