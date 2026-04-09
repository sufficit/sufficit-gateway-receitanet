#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    internal sealed class ReceitaNetMcpTokenResolver
    {
        private readonly IOptions<ReceitaNetMcpOptions> _options;

        public ReceitaNetMcpTokenResolver(IOptions<ReceitaNetMcpOptions> options)
        {
            _options = options;
        }

        public bool TryResolve(HttpContext context, out string token, out string? error)
        {
            var options = _options.Value;

            token = string.Empty;
            error = null;

            if (options.AcceptBearerToken && context.Request.Headers.TryGetValue("Authorization", out var authorizationValues))
            {
                var authorization = authorizationValues.ToString();
                const string prefix = "Bearer ";
                if (authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring(prefix.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(token))
                        return true;
                }
            }

            if (!string.IsNullOrWhiteSpace(options.TokenHeaderName) && context.Request.Headers.TryGetValue(options.TokenHeaderName, out var headerValues))
            {
                token = headerValues.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(token))
                    return true;
            }

            if (options.AcceptQueryStringToken && context.Request.Query.TryGetValue("token", out var queryValues))
            {
                token = queryValues.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(token))
                    return true;
            }

            error = "missing ReceitaNet token; provide Authorization: Bearer <token>, the configured token header, or token query string";
            return false;
        }
    }
}
#endif