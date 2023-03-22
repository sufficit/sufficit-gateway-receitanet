using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public class ContractAndContactParameters : ContractIdParameters
    {
        /// <summary>
        /// (optional) Caller phone
        /// </summary>
        [JsonPropertyName("uracontato")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Contact { get; set; }
    }
}
