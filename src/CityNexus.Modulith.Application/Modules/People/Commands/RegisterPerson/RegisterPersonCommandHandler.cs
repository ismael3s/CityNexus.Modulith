using CityNexus.Modulith.Application.Modules.People.Repositories;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using CityNexus.Modulith.Domain.Modules.People;
using CityNexus.Modulith.Domain.Modules.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;

public sealed class RegisterPersonCommandHandler(
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterPersonCommandHandler> logger
)
{
    public sealed record Input(string Name, string Email, string Document);

    public async Task Handle(Input input, CancellationToken cancellationToken = default!)
    {
        var person = Person.Create(input.Name, input.Email, input.Document);
        var personByDocument = await personRepository.FindByCpf(person.Document, cancellationToken);
        if (personByDocument is not null)
            throw new AppException("CPF is already in use");
        var personByEmail = await personRepository.FindByEmail(person.Email, cancellationToken);
        if (personByEmail is not null)
            throw new AppException("Email is already in use");
        await personRepository.AddAsync(person, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
