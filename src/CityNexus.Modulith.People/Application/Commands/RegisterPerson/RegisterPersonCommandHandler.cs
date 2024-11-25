using CityNexus.Modulith.People.Application.Repositories;
using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.Abstractions;
using CityNexus.Modulith.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CityNexus.Modulith.People.Application.Commands.RegisterPerson;

public sealed class RegisterPersonCommandHandler(
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterPersonCommandHandler> logger
) : IRequestHandler<RegisterPersonCommand>
{
    public async Task Handle(RegisterPersonCommand registerPersonCommand, CancellationToken cancellationToken = default!)
    {
        var person = Person.Create(registerPersonCommand.Name, registerPersonCommand.Email, registerPersonCommand.Document);
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
