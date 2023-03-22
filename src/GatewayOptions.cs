using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class GatewayOptions
    {
        public const string SECTIONNAME = "ReceitaNet";

        public string BaseUrl { get; set; } = "https://sistema.receitanet.net/api/novo/ura/";

        public string ClientId { get; set; } = "ReceitaNet";

        /// <summary>
        /// Default TimeOut (seconds) for endpoints requests 
        /// </summary>
        public uint? TimeOut { get; set; }

        public string Agent { get; set; } = "Sufficit C# API Client";
    }
}
