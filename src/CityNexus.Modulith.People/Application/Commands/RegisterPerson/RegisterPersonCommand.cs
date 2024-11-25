using MediatR;

namespace CityNexus.Modulith.People.Application.Commands.RegisterPerson;

public sealed record RegisterPersonCommand(string Name,string Email, string Document) : IRequest;