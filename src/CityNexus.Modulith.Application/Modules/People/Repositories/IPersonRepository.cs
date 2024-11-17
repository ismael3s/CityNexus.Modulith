using CityNexus.Modulith.Domain.Modules.People;
using CityNexus.Modulith.Domain.Modules.Shared.VO;

namespace CityNexus.Modulith.Application.Modules.People.Repositories;

public interface IPersonRepository
{
    public Task AddAsync(Person people, CancellationToken cancellationToken = default);
    public Task<Person?> FindByCpf(
        Document document,
        CancellationToken cancellationToken = default
    );
    public Task<Person?> FindByEmail(Email email, CancellationToken cancellationToken = default);
}
