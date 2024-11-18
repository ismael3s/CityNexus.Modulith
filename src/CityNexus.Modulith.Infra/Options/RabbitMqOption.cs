namespace CityNexus.Modulith.Infra.Options;

public class RabbitMqOption
{
    public string Host { get; set; } = "localhost";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public ushort Port { get; set; } = 5432;
    public string VHost { get; set; } = "/";
}
