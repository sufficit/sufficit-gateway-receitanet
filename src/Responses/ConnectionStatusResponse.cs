using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class ConnectionStatusResponse : CommonResponse
    {       
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("status")]
        public ConnectionStatus Status { get; set; }

        public enum ConnectionStatus
        {
            Connected = 1,
            Disconnected = 2
        }
    }
}
