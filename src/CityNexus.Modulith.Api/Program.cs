using Asp.Versioning;
using CityNexus.Modulith.Api;
using CityNexus.Modulith.Api.Modules.People;
using CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;
using CityNexus.Modulith.Infra;
using CityNexus.Modulith.People.Infra.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
    .AddInfrastructure(builder.Configuration)
    .AddPeopleModule(builder.Configuration)
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
PeopleModuleRouting.AddPeopleModuleRoutes(apiGroup);

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

app.UseCors();
app.Run();
