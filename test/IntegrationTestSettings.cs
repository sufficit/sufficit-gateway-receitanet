namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal sealed class IntegrationTestSettings
{
    public string BaseUrl { get; set; } = "https://sistema.receitanet.net/api/novo/ura/";

    public string? Token { get; set; }

    public string? Document { get; set; }

    public string? Phone { get; set; }

    public int? ContractId { get; set; }

    public bool AllowSideEffects { get; set; }

    public TicketEnum TicketKind { get; set; } = TicketEnum.Maintenance;

    public string TicketReason { get; set; } = "Integration test requested from Sufficit Gateway.";

    public string RecordingUrl { get; set; } = "https://endpoints.sufficit.com.br/gateway/receitanet/integration-test.wav";

    public int? RecordingExtension { get; set; }

    public bool? RecordingFinalize { get; set; }

    public int? TicketId { get; set; }

    public bool HasToken => !string.IsNullOrWhiteSpace(Token);

    public bool HasDocumentLookup => HasToken && !string.IsNullOrWhiteSpace(Document);

    public bool HasPhoneLookup => HasToken && !string.IsNullOrWhiteSpace(Phone);

    public bool CanResolveContract => ContractId.HasValue || HasDocumentLookup || HasPhoneLookup;
}