using CityNexus.Modulith.People.Domain.Entities.People;
using CityNexus.Modulith.SharedKernel.VO;

namespace CityNexus.Modulith.People.UnitTests.Domain.Entities;

public sealed class PersonTests
{
    [Theory]
    [Trait("People - Unit", "Shouldn't be able to create a person when invalid is provided")]
    [InlineData(null, null, null)]
    [InlineData("Ismael Santana", null, null)]
    [InlineData("Ismael Santana", "ismael.santana@dev.com", null)]
    [InlineData("Ismael Santana", "ismael.santana@dev.com", "123.456.789-12")]
    public void People_Create_Should_Throw_Exception_When_InvalidDataIsProvided(
        string? name,
        string? email,
        string? document
    )
    {
        var action = () => Person.Create(name!, email!, document!);

        action.Should().Throw<Exception>();
    }

    [Theory]
    [Trait("People - Unit", "Should be Able to create a person when valid data is provided")]
    [InlineData("Ismael Santana", "ismael.santana@dev.com", "43807937021")]
    [InlineData("Ismael souza da Silva Santana", "ISMAEL.SANTANA@dev.com", "685.245.620-05")]
    public void People_Create_ShouldBeAbleToCreateAPerson(
        string name,
        string email,
        string document
    )
    {
        var action = () => Person.Create(name, email, document);

        action.Should().NotThrow();
        var person = action();

        person.Should().NotBeNull();
        person.Email.Value.Should().Be(Email.Create(email).Value);
        person.Document.Value.Should().Be(Document.Create(document).Value);
        person.Name.Value.Should().Be(Name.From(name).Value);
    }
}
