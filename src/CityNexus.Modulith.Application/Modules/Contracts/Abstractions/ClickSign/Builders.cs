using System.Text.Json.Serialization;

namespace CityNexus.Modulith.Application.Modules.Contracts.Abstractions.ClickSign;

// This Class is a Huge mess, need to refactor it in a easy way.
// Initial Proposal:
public sealed class Teste<T>
{
    public Teste(T data)
    {
        Data = data;
    }

    public T Data { get; set; }
}

public class EnvelopeRequest
{
    public DataSection Data { get; set; } = new DataSection();

    public class DataSection
    {
        public string Type { get; set; } = "envelopes";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; } = null!;
        public AttributesSection Attributes { get; set; } = new AttributesSection();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, RelationshipSection>? Relationships { get; set; }
    }

    public class AttributesSection
    {
        public string Name { get; set; } = default!;
        public string Locale { get; set; } = "pt-BR";
        public bool AutoClose { get; set; } = true;
        public int RemindInterval { get; set; } = 3;
        public bool BlockAfterRefusal { get; set; } = true;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; set; } = null!;
    }

    public class RelationshipSection
    {
        public DataSection Data { get; set; } = default!;

        public class DataSection
        {
            public string Id { get; set; } = default!;
            public string Type { get; set; } = default!;
        }
    }
}

public sealed class EnvelopeBuilder
{
    private readonly EnvelopeRequest _envelope = new();

    public EnvelopeBuilder SetName(string name)
    {
        _envelope.Data.Attributes.Name = name;
        return this;
    }

    public EnvelopeBuilder SetLocale(string locale)
    {
        _envelope.Data.Attributes.Locale = locale;
        return this;
    }

    public EnvelopeBuilder SetAsRunning()
    {
        _envelope.Data.Attributes.Status = "running";
        return this;
    }

    public EnvelopeBuilder SetId(string id)
    {
        _envelope.Data.Id = id;
        return this;
    }

    public EnvelopeBuilder AddRelationship(string key, string id, string type)
    {
        _envelope.Data.Relationships ??=
            new Dictionary<string, EnvelopeRequest.RelationshipSection>();
        _envelope.Data.Relationships[key] = new EnvelopeRequest.RelationshipSection
        {
            Data = new EnvelopeRequest.RelationshipSection.DataSection { Id = id, Type = type },
        };
        return this;
    }

    public EnvelopeRequest Build()
    {
        return _envelope;
    }
}

public class DocumentRequest
{
    public DataSection Data { get; set; } = new();

    public class DataSection
    {
        public string Type { get; set; } = "documents";
        public AttributesSection Attributes { get; set; } = new();
    }

    public class AttributesSection
    {
        public string Filename { get; set; } = default!;
        public Template Template { get; set; } = default!;
    }

    public record Template
    {
        public string Key { get; init; } = default!;
        public Dictionary<string, string> Data { get; init; } = default!;
        public Dictionary<string, string>? Metadata { get; init; }
    }
}

public class DocumentBuilder
{
    private readonly DocumentRequest _document = new();

    public DocumentBuilder SetFilename(string filename)
    {
        _document.Data.Attributes.Filename = filename;
        return this;
    }

    public DocumentBuilder SetTemplate(
        string key,
        Dictionary<string, string> data,
        Dictionary<string, string>? metadata = null
    )
    {
        _document.Data.Attributes.Template = new DocumentRequest.Template
        {
            Key = key,
            Data = data,
            Metadata = metadata,
        };
        return this;
    }

    public DocumentRequest Build()
    {
        return _document;
    }
}

public class AddSignerRequest(AddSignerRequest.DataSection data)
{
    public DataSection Data { get; set; } = data;

    public class DataSection(string type, AttributesSection attributes)
    {
        public string Type { get; set; } = type;
        public AttributesSection Attributes { get; set; } = attributes;
    }

    public class AttributesSection
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Refusable { get; set; } = false;

        public int Group = 1;

        public AttributesSection(string name, string email, bool refusable, int group)
        {
            Name = name;
            Email = email;
            Refusable = refusable;
            Group = group;
        }
    }
}

public class AddSignerRequestBuilder
{
    private string _type = "signers";
    private string _name = default!;
    private string _email = default!;
    private int _group = 1;
    private bool _refusable = true;

    public AddSignerRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AddSignerRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public AddSignerRequestBuilder WithRefusable(bool refusable)
    {
        _refusable = refusable;
        return this;
    }

    public AddSignerRequestBuilder WithGroup(int group)
    {
        _group = group;
        return this;
    }

    public AddSignerRequest Build()
    {
        var attributes = new AddSignerRequest.AttributesSection(_name, _email, _refusable, _group);
        var data = new AddSignerRequest.DataSection(_type, attributes);
        return new AddSignerRequest(data);
    }
}

public sealed record ClickSignBaseResponse(ClickSignBaseResponse.XData Data)
{
    public sealed record XData(Guid Id);
}

public class AddSignerAuthRequirementRequest(AddSignerAuthRequirementRequest.DataSection data)
{
    public DataSection Data { get; set; } = data;

    public class DataSection(
        string type,
        AttributesSection attributes,
        RelationshipsSection relationships
    )
    {
        public string Type { get; set; } = type;
        public AttributesSection Attributes { get; set; } = attributes;
        public RelationshipsSection Relationships { get; set; } = relationships;
    }

    public class AttributesSection(string action, string auth)
    {
        public string Action { get; set; } = action;
        public string Auth { get; set; } = auth;
    }

    public class RelationshipsSection(RelationshipData document, RelationshipData signer)
    {
        public RelationshipData Document { get; set; } = document;
        public RelationshipData Signer { get; set; } = signer;
    }

    public class RelationshipData(DataItem data)
    {
        public DataItem Data { get; set; } = data;
    }

    public class DataItem(string type, string id)
    {
        public string Type { get; set; } = type;
        public string Id { get; set; } = id;
    }
}

public class AddRequirementRequestBuilder
{
    private string _type = "requirements";
    private string _action = "provide_evidence";
    private string _auth = "email";
    private string _documentId = default!;
    private string _signerId = default!;

    public AddRequirementRequestBuilder WithAction(string action)
    {
        _action = action;
        return this;
    }

    public AddRequirementRequestBuilder WithAuth(string auth)
    {
        _auth = auth;
        return this;
    }

    public AddRequirementRequestBuilder WithDocumentId(string documentId)
    {
        _documentId = documentId;
        return this;
    }

    public AddRequirementRequestBuilder WithSignerId(string signerId)
    {
        _signerId = signerId;
        return this;
    }

    public AddSignerAuthRequirementRequest Build()
    {
        var attributes = new AddSignerAuthRequirementRequest.AttributesSection(_action, _auth);
        var documentRelationship = new AddSignerAuthRequirementRequest.RelationshipData(
            new AddSignerAuthRequirementRequest.DataItem("documents", _documentId)
        );
        var signerRelationship = new AddSignerAuthRequirementRequest.RelationshipData(
            new AddSignerAuthRequirementRequest.DataItem("signers", _signerId)
        );
        var relationships = new AddSignerAuthRequirementRequest.RelationshipsSection(
            documentRelationship,
            signerRelationship
        );
        var data = new AddSignerAuthRequirementRequest.DataSection(
            _type,
            attributes,
            relationships
        );
        return new AddSignerAuthRequirementRequest(data);
    }
}

public class AddSignerAuthRoleRequest
{
    public RequirementsData Data { get; set; } = new RequirementsData();

    public class RequirementsData
    {
        public string Type { get; set; } = "requirements"; // Fixed as per the JSON
        public RequirementsAttributes Attributes { get; set; } = new RequirementsAttributes();
        public RequirementsRelationships Relationships { get; set; } =
            new RequirementsRelationships();
    }

    public class RequirementsAttributes
    {
        public string Action { get; set; } = default!;
        public string Role { get; set; } = default!;
    }

    public class RequirementsRelationships
    {
        public DocumentRelationship Document { get; set; } = default!;
        public SignerRelationship Signer { get; set; } = default!;
    }

    public class DocumentRelationship
    {
        public RelationshipData Data { get; set; } = default!;
    }

    public class SignerRelationship
    {
        public RelationshipData Data { get; set; } = default!;
    }

    public class RelationshipData
    {
        public string Type { get; set; } = default!;
        public string Id { get; set; } = default!;
    }
}

public sealed class RequirementsRequestBuilder
{
    private readonly AddSignerAuthRoleRequest _addSignerAuthRoleRequest = new();

    public RequirementsRequestBuilder SetAction(string action)
    {
        _addSignerAuthRoleRequest.Data.Attributes.Action = action;
        return this;
    }

    public RequirementsRequestBuilder SetRole(string role)
    {
        _addSignerAuthRoleRequest.Data.Attributes.Role = role;
        return this;
    }

    public RequirementsRequestBuilder SetDocument(string documentId)
    {
        _addSignerAuthRoleRequest.Data.Relationships.Document =
            new AddSignerAuthRoleRequest.DocumentRelationship
            {
                Data = new AddSignerAuthRoleRequest.RelationshipData
                {
                    Type = "documents", // Fixed as per the JSON
                    Id = documentId,
                },
            };
        return this;
    }

    public RequirementsRequestBuilder SetSigner(string signerId)
    {
        _addSignerAuthRoleRequest.Data.Relationships.Signer =
            new AddSignerAuthRoleRequest.SignerRelationship
            {
                Data = new AddSignerAuthRoleRequest.RelationshipData
                {
                    Type = "signers", // Fixed as per the JSON
                    Id = signerId,
                },
            };
        return this;
    }

    public AddSignerAuthRoleRequest Build()
    {
        return _addSignerAuthRoleRequest;
    }
}
