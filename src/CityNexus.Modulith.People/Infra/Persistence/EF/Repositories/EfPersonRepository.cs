using CityNexus.Modulith.People.Application.Repositories;
using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.VO;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.Modulith.People.Infra.Persistence.EF.Repositories;

public sealed class EfPersonRepository(PersonDbContext context) : IPersonRepository
{
    public async Task AddAsync(Person people, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(people, cancellationToken);
    }

    public Task<Person?> FindByCpf(Document document, CancellationToken cancellationToken = default)
    {
        return context.People.FirstOrDefaultAsync(p => p.Document == document, cancellationToken);
    }

    public Task<Person?> FindByEmail(Email email, CancellationToken cancellationToken = default)
    {
        return context.People.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }
}
