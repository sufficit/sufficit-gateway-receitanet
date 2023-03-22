using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public class GetContractParameters : ContractIdParameters
    {
        /// <summary>
        /// (optional) identification document as CPF or CPNJ
        /// </summary>
        [JsonPropertyName("cpfcnpj")]
        public string? Document { get; set; }

        /// <summary>
        /// (optional) phone number
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
    }
}
