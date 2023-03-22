using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class RecordingResponse : Response
    {
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("idCliente")]
        public int ClientId { get; set; } = default!;

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}
