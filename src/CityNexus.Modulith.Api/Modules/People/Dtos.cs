using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CityNexus.Modulith.Api.Modules.People;

public sealed record RegisterPersonDto(
    [property: Description("Person Name, must be at least 2 words")]
    [property: Required]
    string Name,
    [property: Required]
    [property: EmailAddress]
    [property: Description("Person Email Address")]
    string Email, 
    [property: Required]
    [property: Description("Brazillian Valid CPF")]
    string Document);