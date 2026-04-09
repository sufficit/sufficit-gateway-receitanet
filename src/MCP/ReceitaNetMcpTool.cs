#if NET7_0_OR_GREATER
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    /// <summary>
    /// Describes one MCP tool exposed by the dedicated ReceitaNet server.
    /// </summary>
    public sealed class ReceitaNetMcpTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("inputSchema")]
        public ReceitaNetMcpToolInputSchema InputSchema { get; set; } = new();
    }

    /// <summary>
    /// Minimal JSON schema used to describe the input payload of a ReceitaNet MCP tool.
    /// </summary>
    public sealed class ReceitaNetMcpToolInputSchema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("properties")]
        public Dictionary<string, ReceitaNetMcpToolProperty> Properties { get; set; } = new();

        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new();
    }

    /// <summary>
    /// Describes one input property inside a ReceitaNet MCP tool schema.
    /// </summary>
    public sealed class ReceitaNetMcpToolProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "string";

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("enum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? EnumValues { get; set; }
    }
}
#endif