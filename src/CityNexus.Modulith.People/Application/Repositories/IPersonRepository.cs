using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.VO;

namespace CityNexus.Modulith.People.Application.Repositories;

public interface IPersonRepository
{
    public Task AddAsync(Person people, CancellationToken cancellationToken = default);
    public Task<Person?> FindByCpf(
        Document document,
        CancellationToken cancellationToken = default
    );
    public Task<Person?> FindByEmail(Email email, CancellationToken cancellationToken = default);
}
