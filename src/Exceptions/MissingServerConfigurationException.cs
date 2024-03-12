using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class MissingServerConfigurationException : HttpRequestException
    {
        public const string MESSAGE = "contract (%CONTRACT%) has no valid server configured at receitanet";

        public int ContractId { get; }

        public MissingServerConfigurationException(int contractid, Exception inner) 
            : base(MESSAGE.Replace("%CONTRACT%", contractid.ToString()), inner) 
            => ContractId = contractid;
    }
}
