using CityNexus.Modulith.SharedKernel.Abstractions;

namespace CityNexus.Modulith.People.Domain.Entities.People.Events;

public sealed record NotificationVariables(string Name, string Value);

public record RegisteredPersonDomainEvent(
    Guid Id,
    string To,
    List<NotificationVariables> Variables,
    string Strategy = "email",
    string Subject = "Seja Bem Vindo!",
    string Template = "welcome"
) : IDomainEvent;
