using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        [JsonPropertyName("vencimento")]
        public string Vencimento { get; set; } = default!;
    }
}
