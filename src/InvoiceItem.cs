using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Sufficit.Json;

namespace Sufficit.Gateway.ReceitaNet
{
    public class InvoiceItem
    {
        /// <summary>
        /// (required)
        /// </summary>
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// (required)
        /// </summary>
        [JsonConverter(typeof(DateConverter))]
        [JsonPropertyName("vencimento")]
        public DateTime Vencimento { get; set; }
    }
}
