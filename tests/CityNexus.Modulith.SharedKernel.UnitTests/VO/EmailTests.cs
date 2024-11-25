
using CityNexus.Modulith.SharedKernel.VO;

namespace CityNexus.Modulith.SharedKernel.UnitTests.VO;

public sealed class EmailTests
{
    [Theory]
    [Trait("Email - Unit Tests", "When invalid email is provided, should throw an exception")]
    [InlineData(null)]
    [InlineData("ismael@  ")]
    [InlineData("ismael123.com")]
    [InlineData("TEST@!@@##!.COM")]
    public void Email_Create_WhenAnInvalidEmail_ShouldThrowAnException(string? value)
    {
        var action = () => Email.Create(value!);

        action.Should().Throw<Exception>().WithMessage("Invalid email address.");
    }

    [Theory]
    [Trait("Email - Unit Tests", "When valid email is provided, shouldn't throw an exception")]
    [InlineData("ismael@gmail.com")]
    [InlineData("TESTE@gmail.com ")]
    public void Email_Create_WhenAnValidEmail_ShouldNotThrowAnException(string value)
    {
        var action = () => Email.Create(value);

        action.Should().NotThrow();
        action().Value.Should().Be(value.Trim().ToLower());
    }
}
