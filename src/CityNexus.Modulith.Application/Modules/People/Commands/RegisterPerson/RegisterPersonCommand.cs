using MediatR;

namespace CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;

public sealed record RegisterPersonCommand(string Name,string Email, string Document) : IRequest;