using CityNexus.Modulith.Domain.Modules.Shared.Abstractions;

namespace CityNexus.Modulith.Domain.Modules.People.Events;

public sealed record NotificationVariables(string Name, string Value);

public record RegisteredPersonDomainEvent(
    Guid Id,
    string To,
    List<NotificationVariables> Variables,
    string Strategy = "email",
    string Subject = "Seja Bem Vindo!",
    string Template = "welcome"
) : IDomainEvent;
