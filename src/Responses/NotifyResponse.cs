using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public class NotifyResponse : ProtocolResponse
    {      
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("status")]
        public NotifyStatus Status { get; set; }

        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("liberado")]
        public bool Allowed { get; set; }

        public enum NotifyStatus
        {
            Allowed = 1,
            Denied = 2
        }
    }
}
