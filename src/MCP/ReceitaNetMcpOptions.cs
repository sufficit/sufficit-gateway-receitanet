#if NET7_0_OR_GREATER
using System;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    /// <summary>
    /// Configuration options for the dedicated ReceitaNet MCP surface.
    /// </summary>
    public sealed class ReceitaNetMcpOptions
    {
        /// <summary>
        /// Configuration section name used to bind MCP-specific settings.
        /// </summary>
        public const string SECTIONNAME = "ReceitaNet:MCP";

        /// <summary>
        /// Base HTTP path where the ReceitaNet MCP endpoints will be mapped.
        /// </summary>
        public string Path { get; set; } = "/mcp/receitanet";

        /// <summary>
        /// Server name returned in MCP discovery responses.
        /// </summary>
        public string ServerName { get; set; } = "Sufficit ReceitaNet MCP";

        /// <summary>
        /// MCP protocol version exposed by the server.
        /// </summary>
        public string ProtocolVersion { get; set; } = "2025-06-18";

        /// <summary>
        /// Custom header name accepted as ReceitaNet token carrier.
        /// </summary>
        public string TokenHeaderName { get; set; } = "X-ReceitaNet-Token";

        /// <summary>
        /// When true, Authorization Bearer values are accepted as ReceitaNet token.
        /// </summary>
        public bool AcceptBearerToken { get; set; } = true;

        /// <summary>
        /// When true, a token query string parameter is accepted.
        /// </summary>
        public bool AcceptQueryStringToken { get; set; } = true;

        /// <summary>
        /// Enables side-effect MCP tools such as ticket, payment, charge and recording updates.
        /// </summary>
        public bool AllowMutatingTools { get; set; } = false;

        /// <summary>
        /// Session ttl in minutes for JSON-RPC MCP clients using the streamable HTTP transport.
        /// </summary>
        public int SessionTtlMinutes { get; set; } = 20;
    }
}
#endif