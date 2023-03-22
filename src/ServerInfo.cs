using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class ServerInfo
    {
        /// <summary>
        /// Indicates that service is in maintenance
        /// </summary>
        [JsonPropertyName("isManutencao")]
        public bool Maintenance { get; set; }

        /// <summary>
        /// Message to client
        /// </summary>
        [JsonPropertyName("mensagemManutencao")]
        public string? Message { get; set; }
    }
}
