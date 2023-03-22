using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public class ChargeNotificationParameters : ContractAndContactParameters
    {
        [JsonPropertyName("tipo")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationChannel Channel { get; set; }
    }
}
