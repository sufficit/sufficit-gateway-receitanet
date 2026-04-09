#if NET7_0_OR_GREATER
using System.ComponentModel;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    /// <summary>
    /// MCP input payload for contract lookup by document.
    /// </summary>
    public sealed class ReceitaNetContractByDocumentRequest
    {
        [Description("CPF or CNPJ document. Accepts both formatted and unformatted values.")]
        public string Document { get; set; } = string.Empty;
    }

    /// <summary>
    /// MCP input payload for contract lookup by phone number.
    /// </summary>
    public sealed class ReceitaNetContractByPhoneRequest
    {
        [Description("Phone number used to search the contract.")]
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// MCP input payload for connection status queries.
    /// </summary>
    public sealed class ReceitaNetConnectionStatusRequest
    {
        [Description("Contract id used to check the current connection status.")]
        public int ContractId { get; set; }

        [Description("Optional contact phone forwarded to ReceitaNet as uracontato.")]
        public string? Contact { get; set; }
    }

    /// <summary>
    /// MCP input payload for charge notification requests.
    /// </summary>
    public sealed class ReceitaNetChargeNotificationRequest
    {
        [Description("Contract id used to send the charge notification.")]
        public int ContractId { get; set; }

        [Description("Notification channel used by ReceitaNet, for example sms or whatsapp.")]
        public NotificationChannel Channel { get; set; }

        [Description("Optional contact phone forwarded to ReceitaNet as uracontato.")]
        public string? Contact { get; set; }
    }

    /// <summary>
    /// MCP input payload for payment notification requests.
    /// </summary>
    public sealed class ReceitaNetPaymentNotificationRequest
    {
        [Description("Contract id used to send the payment notification.")]
        public int ContractId { get; set; }

        [Description("Optional contact phone forwarded to ReceitaNet as uracontato.")]
        public string? Contact { get; set; }
    }

    /// <summary>
    /// MCP input payload for ticket creation requests.
    /// </summary>
    public sealed class ReceitaNetTicketRequest
    {
        [Description("Contract id used to open the support ticket.")]
        public int ContractId { get; set; }

        [Description("Ticket reason or description sent to ReceitaNet.")]
        public string Reason { get; set; } = string.Empty;

        [Description("Ticket kind enum value accepted by ReceitaNet.")]
        public TicketEnum Kind { get; set; }

        [Description("Optional contact phone forwarded to ReceitaNet as uracontato.")]
        public string? Contact { get; set; }
    }

    /// <summary>
    /// MCP input payload for ticket recording updates.
    /// </summary>
    public sealed class ReceitaNetRecordingRequest
    {
        [Description("Support ticket id to update with the recording link.")]
        public int TicketId { get; set; }

        [Description("Public recording URL that ReceitaNet should attach to the ticket.")]
        public string Recording { get; set; } = string.Empty;

        [Description("Optional extension value. Leave null when this project does not use extensions.")]
        public int? Extension { get; set; }

        [Description("Optional flag to request finalization of the ticket after attaching the recording.")]
        public bool? Finalize { get; set; }
    }
}
#endif