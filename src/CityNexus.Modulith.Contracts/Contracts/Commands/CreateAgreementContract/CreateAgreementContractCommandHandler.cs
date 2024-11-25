using CityNexus.Modulith.Application.Modules.Contracts.Abstractions.ClickSign;
using CityNexus.Modulith.Contracts.Contracts.Abstractions.ClickSign;

namespace CityNexus.Modulith.Application.Modules.Contracts.Commands.CreateAgreementContract;

public sealed class CreateAgreementContractCommandHandler(IClickSignGateway clickSignGateway)
{
    public async Task Handle(CancellationToken cancellationToken)
    {
        var fileName = Guid.CreateVersion7().ToString();
        var envelope = new EnvelopeBuilder()
            .SetName($"{fileName}")
            .AddRelationship("folder", "b2f8899a-ac46-46c9-aff9-a672d7d84468", "folders")
            .Build();
        var envelopeResponse = await clickSignGateway.CreateEnvelope(envelope, cancellationToken);
        var document = new DocumentBuilder()
            .SetFilename($"Nome DoCliente.docx")
            .SetTemplate(
                "45b7a1b1-1c36-4302-b4c1-e1679ef37030",
                new Dictionary<string, string> { { "NomeContratante", "AAA.docx" } }
            )
            .Build();
        var signer = new AddSignerRequestBuilder()
            .WithName("Ismael Souza da Silva")
            .WithEmail("souz4ismael@gmail.com")
            .WithGroup(1)
            .Build();
        var signerTwo = new AddSignerRequestBuilder()
            .WithEmail("ismael.santana.dev@gmail.com")
            .WithName("Samuel Souza da Silva Santana")
            .WithGroup(2)
            .Build();
        List<AddSignerRequest> signers = [signer, signerTwo];
        var documentResponse = await clickSignGateway.AddDocumentToEnvelope(
            envelopeResponse.Data.Id.ToString(),
            document,
            cancellationToken
        );
        foreach (var signerToAdd in signers)
        {
            var signerResponse = await clickSignGateway.AddSignerToEnveloper(
                envelopeResponse.Data.Id.ToString(),
                signerToAdd,
                cancellationToken
            );
            var signerId = signerResponse.Data.Id.ToString();
            var requirement = new AddRequirementRequestBuilder()
                .WithAuth("email")
                .WithSignerId(signerId)
                .WithDocumentId(documentResponse.Data.Id.ToString())
                .Build();
            await clickSignGateway.AddSignerAuthMethodToEnvelope(
                envelopeResponse.Data.Id.ToString(),
                requirement,
                cancellationToken
            );
            var addSignerRoleQuest = new RequirementsRequestBuilder()
                .SetAction("agree")
                .SetRole("sign")
                .SetDocument(documentResponse.Data.Id.ToString())
                .SetSigner(signerId)
                .Build();
            await clickSignGateway.AddSignerRoleToEnvelope(
                envelopeResponse.Data.Id.ToString(),
                addSignerRoleQuest,
                cancellationToken
            );
        }
        var updateEnveloper = new EnvelopeBuilder()
            .SetName($"{fileName}")
            .SetAsRunning()
            .SetId(envelopeResponse.Data.Id.ToString())
            .Build();
        await clickSignGateway.ActivateEnvelope(
            envelopeResponse.Data.Id.ToString(),
            updateEnveloper,
            cancellationToken
        );
    }
}
