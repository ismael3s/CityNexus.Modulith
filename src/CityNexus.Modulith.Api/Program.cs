using Asp.Versioning;
using CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;
using CityNexus.Modulith.Application.Modules.People.Commands.RegisterPerson;
using CityNexus.Modulith.Infra;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi().AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

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
);

app.UseCors();
app.Run();
