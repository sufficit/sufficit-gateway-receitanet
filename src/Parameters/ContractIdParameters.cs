using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public abstract class ContractIdParameters : RequestParameters
    {
        /// <summary>
        /// (optional) Contract id
        /// </summary>
        [JsonPropertyName("contrato")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ContractId { get; set; }
    }
}
