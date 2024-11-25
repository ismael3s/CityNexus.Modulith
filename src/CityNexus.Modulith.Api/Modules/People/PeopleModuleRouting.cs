using CityNexus.Modulith.People.Application.Commands.RegisterPerson;
using CityNexus.Modulith.People.Application.Queries.FindPeople;
using CityNexus.Modulith.SharedKernel.Abstractions;
using MediatR;

namespace CityNexus.Modulith.Api.Modules.People;

public static class PeopleModuleRouting
{
    public static void AddPeopleModuleRoutes(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/people").WithTags("People");
        group.MapGet(
                "/",
                async (
                    ISender sender,
                    CancellationToken ct,
                    int page = 1,
                    int size = 10
                ) =>
                {
                    var result = await sender.Send(new FindPeopleQuery(page, size), ct);
                    return Results.Ok(result);
                }
            )
            .WithDescription("Search for people")
            .Produces<Pagination<FindPeopleQueryHandler.Output>>(StatusCodes.Status200OK);

        group.MapPost(
                "/",
                async (
                    RegisterPersonDto input,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    await sender.Send(new RegisterPersonCommand(Name: input.Name, Email: input.Email, Document: input.Document), ct);
                    return Results.NoContent();
                }
            )
            .WithDescription("Register a person into the nexus")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;
    }
}