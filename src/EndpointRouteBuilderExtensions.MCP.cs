#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.MCP;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    /// <summary>
    /// HTTP endpoint mapping helpers for the dedicated ReceitaNet MCP surface.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        private const string JsonRpcVersion = "2.0";
        private const string SessionHeaderName = "mcp-session-id";
        private static readonly string[] SupportedProtocolVersions = new[] { "2025-06-18", "2025-03-26", "2024-11-05" };

        /// <summary>
        /// Maps the ReceitaNet MCP discovery and tool invocation endpoints.
        /// </summary>
        /// <param name="endpoints">Application endpoint route builder.</param>
        /// <param name="configure">Optional callback to override route-time MCP options such as the base path.</param>
        /// <returns>The same endpoint route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapGatewayReceitaNetMCP(this IEndpointRouteBuilder endpoints, Action<ReceitaNetMcpOptions>? configure = null)
        {
            var options = CloneOptions(endpoints.ServiceProvider.GetRequiredService<IOptions<ReceitaNetMcpOptions>>().Value);
            configure?.Invoke(options);
            var group = endpoints.MapGroup(options.Path).WithTags("ReceitaNet MCP");

            group.MapGet(string.Empty, (ReceitaNetMcpToolCatalog catalog) => Results.Json(new
            {
                name = options.ServerName,
                version = typeof(APIClientService).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                protocolVersion = options.ProtocolVersion,
                capabilities = new
                {
                    tools = new { listChanged = false },
                },
                tools = catalog.GetTools(),
            }, Json.Options));

            group.MapPost(string.Empty, async Task<IResult> (
                HttpContext context,
                APIClientService client,
                ReceitaNetMcpToolCatalog catalog,
                ReceitaNetMcpTokenResolver tokenResolver,
                ReceitaNetMcpSessionManager sessionManager,
                CancellationToken cancellationToken) =>
            {
                var message = await ReadArgumentsAsync(context, cancellationToken);
                if (message.ValueKind != JsonValueKind.Object)
                    return CreateErrorResponse(null, -32700, "Parse error", StatusCodes.Status400BadRequest);

                if (!message.TryGetProperty("method", out var methodElement))
                    return CreateErrorResponse(GetJsonRpcId(message), -32700, "Parse error", StatusCodes.Status400BadRequest);

                var method = methodElement.GetString();
                if (string.IsNullOrWhiteSpace(method))
                    return CreateErrorResponse(GetJsonRpcId(message), -32700, "Parse error", StatusCodes.Status400BadRequest);

                var requestId = GetJsonRpcId(message);
                var parameters = message.TryGetProperty("params", out var paramsElement)
                    ? paramsElement.Clone()
                    : CreateEmptyObject();

                var sessionId = context.Request.Headers[SessionHeaderName].ToString();
                switch (method)
                {
                    case "initialize":
                    {
                        var initializedSessionId = sessionManager.Initialize(
                            sessionId,
                            GetClientInfoValue(parameters, "name"),
                            GetClientInfoValue(parameters, "version"));

                        context.Response.Headers[SessionHeaderName] = initializedSessionId;
                        return CreateResultResponse(requestId, new
                        {
                            protocolVersion = NegotiateProtocolVersion(options.ProtocolVersion, context, parameters),
                            capabilities = new
                            {
                                tools = new { listChanged = false },
                            },
                            serverInfo = new
                            {
                                name = options.ServerName,
                                version = typeof(APIClientService).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                            }
                        });
                    }
                    case "notifications/initialized":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        return Results.NoContent();
                    }
                    case "tools/list":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        return CreateResultResponse(requestId, new
                        {
                            tools = catalog.GetTools(),
                        });
                    }
                    case "tools/call":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        if (!tokenResolver.TryResolve(context, out var token, out var error))
                            return CreateErrorResponse(requestId, -32001, error, StatusCodes.Status401Unauthorized);

                        if (!parameters.TryGetProperty("name", out var nameElement))
                            return CreateErrorResponse(requestId, -32602, "Invalid params: missing name", StatusCodes.Status400BadRequest);

                        var toolName = nameElement.GetString();
                        if (string.IsNullOrWhiteSpace(toolName))
                            return CreateErrorResponse(requestId, -32602, "Invalid params: missing name", StatusCodes.Status400BadRequest);

                        var arguments = parameters.TryGetProperty("arguments", out var argsElement)
                            ? argsElement.Clone()
                            : CreateEmptyObject();

                        try
                        {
                            var result = await catalog.InvokeAsync(toolName, arguments, client, token, cancellationToken);
                            var text = result is string textResult
                                ? textResult
                                : JsonSerializer.Serialize(result, Json.Options);

                            return CreateResultResponse(requestId, new
                            {
                                content = new[]
                                {
                                    new
                                    {
                                        type = "text",
                                        text,
                                    }
                                }
                            });
                        }
                        catch (InvalidOperationException ex)
                        {
                            return CreateErrorResponse(requestId, -32603, ex.Message, StatusCodes.Status400BadRequest);
                        }
                    }
                    case "logging/setLevel":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        return CreateResultResponse(requestId, new { });
                    }
                    case "prompts/list":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        return CreateResultResponse(requestId, new
                        {
                            prompts = Array.Empty<object>(),
                        });
                    }
                    case "ping":
                    {
                        if (!sessionManager.Validate(sessionId))
                            return CreateErrorResponse(requestId, -32000, "Session not found or expired. Please reconnect using the 'initialize' method.", StatusCodes.Status401Unauthorized);

                        return CreateResultResponse(requestId, new { });
                    }
                    default:
                        return CreateErrorResponse(requestId, -32601, "Method not found", StatusCodes.Status404NotFound);
                }
            });

            group.MapGet("/tools", (ReceitaNetMcpToolCatalog catalog) => Results.Json(catalog.GetTools(), Json.Options));

            group.MapPost("/tools/{toolName}", async Task<IResult> (string toolName, HttpContext context, APIClientService client, ReceitaNetMcpTokenResolver tokenResolver, ReceitaNetMcpToolCatalog catalog, CancellationToken cancellationToken) =>
            {
                if (!tokenResolver.TryResolve(context, out var token, out var error))
                    return Results.Json(new { success = false, message = error }, Json.Options, statusCode: StatusCodes.Status401Unauthorized);

                try
                {
                    var arguments = await ReadArgumentsAsync(context, cancellationToken);
                    var result = await catalog.InvokeAsync(toolName, arguments, client, token, cancellationToken);
                    return Results.Json(new { success = true, tool = toolName, result }, Json.Options);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Json(new { success = false, tool = toolName, message = ex.Message }, Json.Options, statusCode: StatusCodes.Status400BadRequest);
                }
            });

            return endpoints;
        }

        private static ReceitaNetMcpOptions CloneOptions(ReceitaNetMcpOptions source)
        {
            return new ReceitaNetMcpOptions()
            {
                Path = source.Path,
                ServerName = source.ServerName,
                ProtocolVersion = source.ProtocolVersion,
                TokenHeaderName = source.TokenHeaderName,
                AcceptBearerToken = source.AcceptBearerToken,
                AcceptQueryStringToken = source.AcceptQueryStringToken,
                AllowMutatingTools = source.AllowMutatingTools,
                SessionTtlMinutes = source.SessionTtlMinutes,
            };
        }

        private static IResult CreateResultResponse(object? id, object result)
            => Results.Json(new
            {
                jsonrpc = JsonRpcVersion,
                id,
                result,
            }, Json.Options);

        private static IResult CreateErrorResponse(object? id, int code, string? message, int statusCode)
            => Results.Json(new
            {
                jsonrpc = JsonRpcVersion,
                id,
                error = new
                {
                    code,
                    message = string.IsNullOrWhiteSpace(message) ? "Unknown error" : message,
                }
            }, Json.Options, statusCode: statusCode);

        private static object? GetJsonRpcId(JsonElement message)
        {
            if (!message.TryGetProperty("id", out var idElement))
                return null;

            return idElement.Clone();
        }

        private static JsonElement CreateEmptyObject()
        {
            using var emptyDocument = JsonDocument.Parse("{}");
            return emptyDocument.RootElement.Clone();
        }

        private static string NegotiateProtocolVersion(string serverVersion, HttpContext context, JsonElement parameters)
        {
            var requested = context.Request.Headers["mcp-protocol-version"].ToString();
            if (string.IsNullOrWhiteSpace(requested) && parameters.ValueKind == JsonValueKind.Object && parameters.TryGetProperty("protocolVersion", out var protocolElement))
                requested = protocolElement.GetString();

            if (!string.IsNullOrWhiteSpace(requested) && Array.Exists(SupportedProtocolVersions, item => string.Equals(item, requested, StringComparison.OrdinalIgnoreCase)))
                return requested;

            return serverVersion;
        }

        private static string? GetClientInfoValue(JsonElement parameters, string propertyName)
        {
            if (parameters.ValueKind != JsonValueKind.Object)
                return null;

            if (!parameters.TryGetProperty("clientInfo", out var clientInfoElement) || clientInfoElement.ValueKind != JsonValueKind.Object)
                return null;

            if (!clientInfoElement.TryGetProperty(propertyName, out var valueElement))
                return null;

            return valueElement.GetString();
        }

        private static async Task<JsonElement> ReadArgumentsAsync(HttpContext context, CancellationToken cancellationToken)
        {
            if (context.Request.ContentLength.GetValueOrDefault() <= 0)
            {
                using var emptyDocument = JsonDocument.Parse("{}");
                return emptyDocument.RootElement.Clone();
            }

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var raw = await reader.ReadToEndAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(raw))
            {
                using var emptyDocument = JsonDocument.Parse("{}");
                return emptyDocument.RootElement.Clone();
            }

            using var document = JsonDocument.Parse(raw);
            return document.RootElement.Clone();
        }
    }
}
#endif