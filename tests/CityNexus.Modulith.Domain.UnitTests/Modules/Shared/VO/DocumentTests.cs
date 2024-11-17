using CityNexus.Modulith.Domain.Modules.Shared.VO;

namespace CityNexus.Modulith.Domain.UnitTests.Modules.Shared.VO;

public sealed class DocumentTests
{
    [Theory]
    [Trait("Document - Unit", "When Invalid document data is provided, should throws Exceptions")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("123.456.789-20")]
    [InlineData("023.412.789-20")]
    public void Document_Create_WhenInvalidDataIsPassed_ShouldThrowException(string? value)
    {
        var action = () => Document.Create(value!);

        action.Should().Throw<Exception>().WithMessage("The CPF is invalid");
    }

    [Theory]
    [Trait("Document - Unit", "When a valid CPF data is provided, shouldn't throws Exceptions")]
    [InlineData("14195386080")]
    [InlineData("44055468008")]
    [InlineData("44055468008 ")]
    [InlineData("709.200.460-88")]
    [InlineData("338.124.560-01 ")]
    public void Document_Create_ValidDataIsPassed_ShouldNotThrowsException(string value)
    {
        var action = () => Document.Create(value);

        action.Should().NotThrow();
        action().Value.Should().Be(value.Trim().Replace("-", "").Replace(".", ""));
    }
}
