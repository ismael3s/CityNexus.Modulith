using System.ComponentModel.DataAnnotations;

namespace CityNexus.Modulith.Infra.Options;

public sealed class ClickSignOption
{
    [Required]
    public string ApiKey { get; set; } = default!;

    [Required]
    [Url]
    public string ApiUrl { get; set; } = default!;
}
