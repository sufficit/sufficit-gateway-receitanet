using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public abstract class CommonResponse : Response
    {
        /// <summary>
        /// Official ReceitaNet customer identifier.
        /// </summary>
        [JsonPropertyName("idCliente")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ClientId { get; set; } = default!;

        /// <summary>
        /// Official ReceitaNet contract identifier used by contract-scoped operations.
        /// </summary>
        [JsonPropertyName("contratoId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public int? ContractId { get; set; }

        /// <summary>
        /// Uses the official contract identifier when available and falls back to the customer identifier only for legacy payload compatibility.
        /// </summary>
        [JsonIgnore]
        public int EffectiveContractId
            => ContractId.HasValue && ContractId.Value > 0 ? ContractId.Value : ClientId;

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("cpfCnpj")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Document { get; set; } = default!;

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("razaoSocial")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Title { get; set; } = default!;

        /// <summary>
        /// (optional) Caller phone
        /// </summary>
        [JsonPropertyName("uracontato")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public string? Contact { get; set; }
    }
}
