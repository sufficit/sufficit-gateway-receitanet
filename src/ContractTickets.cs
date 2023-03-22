using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class ContractTickets
    {
        [JsonPropertyName("1")]
        public int Primeiro { get; set; }

        [JsonPropertyName("2")]
        public int Segundo { get; set; }

        [JsonPropertyName("3")]
        public int Terceiro { get; set; }

        [JsonPropertyName("4")]
        public int Quarto { get; set; }

        [JsonPropertyName("5")]
        public int Quinto { get; set; }        
    }
}
