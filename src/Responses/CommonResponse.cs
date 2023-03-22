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
        /// (required)
        /// </summary>
        [JsonPropertyName("idCliente")]
        public int ClientId { get; set; } = default!;

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("contratoId")]
        [Obsolete]
        public int? ContractId { get; set; }

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("cpfCnpj")]
        public string Document { get; set; } = default!;

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("razaoSocial")]
        public string Title { get; set; } = default!;

        /// <summary>
        /// (optional) Caller phone
        /// </summary>
        [JsonPropertyName("uracontato")]
        public string? Contact { get; set; }
    }
}
