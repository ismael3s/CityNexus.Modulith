namespace CityNexus.Modulith.Infra.Persistence.EF;

public sealed class Outbox
{
    public Guid Id { get; private set; }

    public string EventName { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public string? Error { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    private Outbox() { }

    public Outbox(string name, string payload, string? error = null)
    {
        Id = Guid.CreateVersion7();
        EventName = name;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
        Error = error;
    }
}
