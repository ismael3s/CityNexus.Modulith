using CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;
using CityNexus.Modulith.Application.Modules.People.Queries.FindPeople;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
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
                    RegisterPersonCommand input,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    await sender.Send(input, ct);
                    return Results.NoContent();
                }
            )
            .WithDescription("Register a person into the nexus")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;
    }

    
}