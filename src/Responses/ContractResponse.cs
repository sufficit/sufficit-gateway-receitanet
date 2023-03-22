using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class ContractResponse : Response
    {
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("contratos")]
        public Contract? Contract { get; set; }
    }
}
