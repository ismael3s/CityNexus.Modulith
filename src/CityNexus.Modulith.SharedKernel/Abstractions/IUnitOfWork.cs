namespace CityNexus.Modulith.SharedKernel.Abstractions;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}
