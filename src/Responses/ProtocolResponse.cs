using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public abstract class ProtocolResponse : CommonResponse
    {
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("protocolo")]
        public string Protocol { get; set; } = default!;
    }
}
