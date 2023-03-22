using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class ChargeResponse : ProtocolResponse
    {
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}
