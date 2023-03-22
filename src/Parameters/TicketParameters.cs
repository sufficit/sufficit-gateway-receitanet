using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public class TicketParameters : ContractAndContactParameters
    {
        [JsonPropertyName("motivoos")]
        public string Reason { get; set; } = default!;

        [JsonPropertyName("ocorrenciatipo")]
        public TicketKind Kind { get; set; }

        public enum TicketKind
        {
            Maintenance = 1,
            Installation = 2,
            EquipmentRemoval = 4,
            Financial = 5
        }
    }
}
