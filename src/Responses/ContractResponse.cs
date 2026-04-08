using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    [JsonConverter(typeof(ContractResponseJsonConverter))]
    public class ContractResponse : Response
    {
        [JsonIgnore]
        public IReadOnlyList<Contract> Contracts { get; internal set; } = Array.Empty<Contract>();

        [JsonIgnore]
        public Contract? Contract => Contracts.FirstOrDefault();

        [JsonIgnore]
        public bool HasMultipleContracts => Contracts.Count > 1;
    }
}
