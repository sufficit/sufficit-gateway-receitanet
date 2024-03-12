using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet
{
    public class ContractException : Exception
    {
        [JsonPropertyName("contract")]
        public Contract Contract { get => (Contract)Data["contract"]!; }

        public ContractException(Contract contract, string message) : base(message)
        {
            Data["contract"] = contract;
        }
    }
}
