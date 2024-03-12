using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Sufficit.Gateway.ReceitaNet
{
    public class MissingChargeMethodException : ContractException
    {
        public const string MESSAGE = "contract needs a valid charge method";

        public MissingChargeMethodException(Contract contract) : base(contract, MESSAGE) { }
    }
}
