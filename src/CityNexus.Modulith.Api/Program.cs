using Asp.Versioning;
using CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;
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
        // var fileName = Guid.CreateVersion7().ToString();
        // var envelope = new EnvelopeBuilder()
        //     .SetName($"{fileName}")
        //     .AddRelationship("folder", "b2f8899a-ac46-46c9-aff9-a672d7d84468", "folders")
        //     .Build();
        // var envelopeResponse = await ghService.CreateEnvelope(
        //     envelope, ct
        //     );
        // var document = new DocumentBuilder().SetFilename("Pessoa.csv")
        //     .SetFilename($"Nome DoCliente.docx")
        //     .SetTemplate(
        //         "45b7a1b1-1c36-4302-b4c1-e1679ef37030",
        //         new Dictionary<string, string>
        //         {
        //             { "NomeContratante", "AAA.docx" },
        //         }
        //         )
        //     .Build();
        // var signer = new AddSignerRequestBuilder()
        //     .WithName("Ismael Souza da Silva")
        //     .WithEmail("souz4ismael@gmail.com")
        //     .WithGroup(1)
        //     .Build();
        // var signerTwo = new AddSignerRequestBuilder().WithEmail("ismael.santana.dev@gmail.com")
        //     .WithName("Samuel Souza da Silva Santana")
        //     .WithGroup(2)
        //     .Build();
        // List<AddSignerRequest> signers = [signer, signerTwo];
        // var documentResponse = await ghService.AddDocumentToEnvelope(envelopeResponse.Data.Id.ToString(),  document, ct);
        // foreach (var signerToAdd in signers)
        // {
        //     var signerResponse = await ghService.AddSignerToEnveloper(envelopeResponse.Data.Id.ToString(), signerToAdd, ct);
        //     var signerId = signerResponse.Data.Id.ToString();
        //     var requirement = new AddRequirementRequestBuilder().WithAuth("email")
        //         .WithSignerId(signerId)
        //         .WithDocumentId(documentResponse.Data.Id.ToString())
        //         .Build();
        //     await ghService.AddSignerAuthMethodToEnvelope(envelopeResponse.Data.Id.ToString(), requirement, ct);
        //     var requirementAAA = new RequirementsRequestBuilder()
        //         .SetAction("agree")
        //         .SetRole("sign")
        //         .SetDocument(documentResponse.Data.Id.ToString())
        //         .SetSigner(signerId)
        //         .Build();
        //     await ghService.AddSignerRoleToEnvelope(envelopeResponse.Data.Id.ToString(), requirementAAA, ct);
        //     // await ghService.AddSignerToEnveloper(envelopeResponse.Data.Id.ToString(), requirementAAA, ct);
        //     // await ghService.AddSignerRoleToEnvelope(envelopeResponse.Data.Id.ToString(), document, ct);
        // }
        //
        // var updateEnveloper = new EnvelopeBuilder()
        //     .SetName($"{fileName}")
        //     .SetAsRunning()
        //     .SetId(envelopeResponse.Data.Id.ToString())
        //     .Build();
        // await ghService.ActivateEnvelope(envelopeResponse.Data.Id.ToString(), updateEnveloper, ct);
    }
);

app.UseCors();
app.Run();
