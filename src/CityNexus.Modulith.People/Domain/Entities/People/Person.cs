using CityNexus.Modulith.People.Domain.Entities.People.Events;
using CityNexus.Modulith.SharedKernel.Abstractions;
using CityNexus.Modulith.SharedKernel.VO;

namespace CityNexus.Modulith.People.Domain.Entities.People;

public sealed class Person : Entity
{
    public Name Name { get; private set; } = default!;
    public Document Document { get; private set; } = default!;
    public Email Email { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Person() { }

    public static Person Create(string fullName, string anEmail, string cpf)
    {
        var document = Document.Create(cpf);
        var email = Email.Create(anEmail);
        var name = Name.From(fullName);
        var person = new Person
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Document = document,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        person.RaiseDomainEvent(
            new RegisteredPersonDomainEvent(
                Id: person.Id,
                To: person.Email.Value,
                [new("name", name.Value), new("email", email.Value)]
            )
        );
        return person;
    }
}
