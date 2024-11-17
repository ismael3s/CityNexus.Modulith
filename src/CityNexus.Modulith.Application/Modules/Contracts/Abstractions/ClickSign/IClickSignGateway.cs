using Refit;

namespace CityNexus.Modulith.Application.Modules.Contracts.Abstractions.ClickSign;

public interface IClickSignGateway
{
    [Post("/api/v3/envelopes")]
    [Headers("Content-Type: application/vnd.api+json")]
    public Task<ClickSignBaseResponse> CreateEnvelope(
        EnvelopeRequest input,
        CancellationToken ct = default
    );

    [Patch("/api/v3/envelopes/{envelopeId}")]
    [Headers("Content-Type: application/vnd.api+json")]
    Task ActivateEnvelope(string envelopeId, EnvelopeRequest input, CancellationToken ct);

    [Get("/api/v3/envelopes")]
    public Task<object> FindEnvelopes(CancellationToken ct = default);

    [Post("/api/v3/envelopes/{envelopeId}/documents")]
    [Headers("Content-Type: application/vnd.api+json")]
    public Task<ClickSignBaseResponse> AddDocumentToEnvelope(
        string envelopeId,
        DocumentRequest input,
        CancellationToken ct = default
    );

    [Post("/api/v3/envelopes/{envelopeId}/signers")]
    [Headers("Content-Type: application/vnd.api+json")]
    public Task<ClickSignBaseResponse> AddSignerToEnveloper(
        string envelopeId,
        AddSignerRequest input,
        CancellationToken ct = default
    );

    [Post("/api/v3/envelopes/{envelopeId}/requirements")]
    [Headers("Content-Type: application/vnd.api+json")]
    public Task<ClickSignBaseResponse> AddSignerAuthMethodToEnvelope(
        string envelopeId,
        AddSignerAuthRequirementRequest input,
        CancellationToken ct = default
    );

    [Post("/api/v3/envelopes/{envelopeId}/requirements")]
    [Headers("Content-Type: application/vnd.api+json")]
    public Task<ClickSignBaseResponse> AddSignerRoleToEnvelope(
        string envelopeId,
        AddSignerAuthRoleRequest input,
        CancellationToken ct = default
    );
}
