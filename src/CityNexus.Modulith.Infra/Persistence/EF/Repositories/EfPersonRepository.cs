using CityNexus.Modulith.Application.Modules.People.Repositories;
using CityNexus.Modulith.Domain.Modules.People;
using CityNexus.Modulith.Domain.Modules.Shared.VO;
using Microsoft.EntityFrameworkCore;

namespace CityNexus.Modulith.Infra.Persistence.EF.Repositories;

public sealed class EfPersonRepository(ApplicationDbContext context) : IPersonRepository
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
