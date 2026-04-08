using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Parameters
{
    public abstract class RequestParameters
    {
        /// <summary>
        /// Application identifier expected by ReceitaNet API.
        /// </summary>
        [JsonPropertyName("app")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Application { get; set; }
    }
}