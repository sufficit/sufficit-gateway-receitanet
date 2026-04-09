#if NET7_0_OR_GREATER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    internal sealed class ReceitaNetMcpToolCatalog
    {
        private sealed record Definition(string Name, string Description, Type InputType, bool Mutating);

        private readonly ILogger<ReceitaNetMcpToolCatalog> _logger;
        private readonly IOptions<ReceitaNetMcpOptions> _options;
        private readonly IReadOnlyDictionary<string, Definition> _definitions;

        public ReceitaNetMcpToolCatalog(ILogger<ReceitaNetMcpToolCatalog> logger, IOptions<ReceitaNetMcpOptions> options)
        {
            _logger = logger;
            _options = options;
            _definitions = new Dictionary<string, Definition>(StringComparer.OrdinalIgnoreCase)
            {
                ["receitanet_contract_by_document"] = new("receitanet_contract_by_document", "Get contract data by CPF or CNPJ document.", typeof(ReceitaNetContractByDocumentRequest), false),
                ["receitanet_contract_by_phone"] = new("receitanet_contract_by_phone", "Get contract data by phone number.", typeof(ReceitaNetContractByPhoneRequest), false),
                ["receitanet_connection_status"] = new("receitanet_connection_status", "Check current connection status for one contract.", typeof(ReceitaNetConnectionStatusRequest), false),
                ["receitanet_charge_notification"] = new("receitanet_charge_notification", "Send a charge notification to one contract.", typeof(ReceitaNetChargeNotificationRequest), true),
                ["receitanet_payment_notification"] = new("receitanet_payment_notification", "Send a payment notification to one contract.", typeof(ReceitaNetPaymentNotificationRequest), true),
                ["receitanet_ticket"] = new("receitanet_ticket", "Open a support ticket for one contract.", typeof(ReceitaNetTicketRequest), true),
                ["receitanet_recording"] = new("receitanet_recording", "Attach a recording URL to one support ticket.", typeof(ReceitaNetRecordingRequest), true),
            };
        }

        public IEnumerable<ReceitaNetMcpTool> GetTools()
            => _definitions.Values.Select(definition => new ReceitaNetMcpTool()
            {
                Name = definition.Name,
                Description = definition.Description,
                InputSchema = ReceitaNetMcpSchemaBuilder.Build(definition.InputType),
            });

        public async Task<object?> InvokeAsync(string toolName, JsonElement arguments, APIClientService client, string token, CancellationToken cancellationToken)
        {
            if (!_definitions.TryGetValue(toolName, out var definition))
                throw new InvalidOperationException($"unknown ReceitaNet MCP tool: {toolName}");

            if (definition.Mutating && !_options.Value.AllowMutatingTools)
                throw new InvalidOperationException($"tool '{toolName}' is disabled; enable ReceitaNet:MCP:AllowMutatingTools to execute side-effect operations");

            _logger.LogInformation("executing receitanet mcp tool: {tool}", toolName);

            switch (toolName)
            {
                case "receitanet_contract_by_document":
                {
                    var request = Deserialize<ReceitaNetContractByDocumentRequest>(arguments);
                    if (string.IsNullOrWhiteSpace(request.Document))
                        throw new InvalidOperationException("document is required");

                    return await client.GetContractByDocument(token, request.Document, cancellationToken);
                }
                case "receitanet_contract_by_phone":
                {
                    var request = Deserialize<ReceitaNetContractByPhoneRequest>(arguments);
                    if (string.IsNullOrWhiteSpace(request.Phone))
                        throw new InvalidOperationException("phone is required");

                    return await client.GetContractByPhone(token, request.Phone, cancellationToken);
                }
                case "receitanet_connection_status":
                {
                    var request = Deserialize<ReceitaNetConnectionStatusRequest>(arguments);
                    var parameters = new ContractAndContactParameters()
                    {
                        ContractId = request.ContractId,
                        Contact = request.Contact,
                    };

                    return await client.GetConnectionStatus(parameters, token, cancellationToken);
                }
                case "receitanet_charge_notification":
                {
                    var request = Deserialize<ReceitaNetChargeNotificationRequest>(arguments);
                    var parameters = new ChargeNotificationParameters()
                    {
                        ContractId = request.ContractId,
                        Contact = request.Contact,
                        Channel = request.Channel,
                    };

                    return await client.ChargeNotification(parameters, token, cancellationToken);
                }
                case "receitanet_payment_notification":
                {
                    var request = Deserialize<ReceitaNetPaymentNotificationRequest>(arguments);
                    var parameters = new ContractAndContactParameters()
                    {
                        ContractId = request.ContractId,
                        Contact = request.Contact,
                    };

                    return await client.PaymentNotification(parameters, token, cancellationToken);
                }
                case "receitanet_ticket":
                {
                    var request = Deserialize<ReceitaNetTicketRequest>(arguments);
                    if (string.IsNullOrWhiteSpace(request.Reason))
                        throw new InvalidOperationException("reason is required");

                    var parameters = new TicketParameters()
                    {
                        ContractId = request.ContractId,
                        Contact = request.Contact,
                        Kind = request.Kind,
                        Reason = request.Reason,
                    };

                    return await client.Ticket(parameters, token, cancellationToken);
                }
                case "receitanet_recording":
                {
                    var request = Deserialize<ReceitaNetRecordingRequest>(arguments);
                    if (string.IsNullOrWhiteSpace(request.Recording))
                        throw new InvalidOperationException("recording is required");

                    var parameters = new RecordingParameters()
                    {
                        TicketId = request.TicketId,
                        Recording = request.Recording,
                        Extension = request.Extension,
                        Finalize = request.Finalize,
                    };

                    return await client.Recording(parameters, token, cancellationToken);
                }
                default:
                    throw new InvalidOperationException($"unsupported ReceitaNet MCP tool: {toolName}");
            }
        }

        private static T Deserialize<T>(JsonElement arguments)
        {
            var deserialized = arguments.Deserialize<T>(Json.Options);
            if (deserialized == null)
                throw new InvalidOperationException($"invalid or empty arguments for {typeof(T).Name}");

            return deserialized;
        }
    }
}
#endif