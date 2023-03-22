using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public class RecordingParameters 
    {
        [JsonPropertyName("idSuporte")]
        public int TicketId { get; set; } = default!;

        /// <summary>
        /// Recording URL
        /// </summary>
        [JsonPropertyName("urlgravacao")]
        public string Recording { get; set; } = default!;

        [JsonPropertyName("ramal")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        public int? Extension { get; set; }

        /// <summary>
        /// Should finalize ticket
        /// </summary>
        [JsonPropertyName("is_finalizar")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Finalize { get; set; }
    }
}
