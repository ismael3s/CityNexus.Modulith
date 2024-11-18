using System.Net;
using Asp.Versioning;
using CityNexus.Modulith.Api;
using CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;
using CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;
using CityNexus.Modulith.Application.Modules.People.Queries.FindPeople;
using CityNexus.Modulith.Application.Modules.Shared.Abstractions;
using CityNexus.Modulith.Infra;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
    .AddInfrastructure(builder.Configuration)
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var apiGroup = app.MapGroup("/api").WithApiVersionSet(apiVersionSet);

apiGroup.MapHealthChecks(
    "/healthz",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

apiGroup.MapGet(
    "/",
    async (CreateAgreementContractCommandHandler handler, CancellationToken ct) =>
    {
        await handler.Handle(ct);
    }
);

apiGroup.MapPost(
    "/people",
    async (
        RegisterPersonCommandHandler.Input input,
        CancellationToken ct,
        RegisterPersonCommandHandler handler
    ) =>
    {
        await handler.Handle(input, ct);
        return Results.NoContent();
    }
)
.WithDescription("Register a person into the nexus")
.WithTags("People")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest)
    ;

apiGroup.MapGet(
    "/people",
    async (
     
        FindPeopleQueryHandler handler,
        CancellationToken ct,
        int page = 1,
        int size = 10
    ) =>
    {
        var result = await handler.Handle(new FindPeopleQueryHandler.Input(page, size), ct);
        return Results.Ok(result);
    }
)
.WithDescription("Search for people")
.WithTags("People")
.Produces<Pagination<FindPeopleQueryHandler.Output>>(StatusCodes.Status200OK)
;
    

app.UseCors();
app.Run();
