using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public abstract class ControllerSection
    {
        protected readonly IOptionsMonitor<GatewayOptions> ioptions;
        protected readonly IHttpClientFactory factory;
        protected readonly ILogger logger;
        protected readonly JsonSerializerOptions jsonOptions;

        public ControllerSection(IOptionsMonitor<GatewayOptions> ioptions, IHttpClientFactory factory, ILogger logger, JsonSerializerOptions jsonOptions)
        {
            this.ioptions = ioptions;
            this.factory = factory;
            this.logger = logger;
            this.jsonOptions = jsonOptions;
        }

        public ControllerSection(APIClientService service)
        {
            ioptions = service.ioptions;
            factory = service.factory;
            logger = service.logger;
            jsonOptions = service.jsonOptions;
        }

        #region TRICKS 

        protected HttpClient httpClient
            => factory.Configure(options);

        protected GatewayOptions options
            => ioptions.CurrentValue;

        #endregion

        protected async Task<IEnumerable<T>> RequestMany<T>(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<T>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>(jsonOptions, cancellationToken) ?? Array.Empty<T>();
        }

        protected async Task<T> Request<T>(HttpRequestMessage message, CancellationToken cancellationToken, bool readNotFoundBody = true, string? notFoundMessage = null, bool readBadRequestBody = false, string? badRequestMessage = null) where T : Response, new()
        {
            using var response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                if (readNotFoundBody)
                {
                    var notFoundBody = await TryReadResponse<T>(response, cancellationToken);
                    if (notFoundBody != null) return notFoundBody;
                }

                return new T() { Success = false, Message = notFoundMessage ?? "not found" };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest && readBadRequestBody)
            {
                var badRequestBody = await TryReadResponse<T>(response, cancellationToken);
                if (badRequestBody != null) return badRequestBody;

                return new T() { Success = false, Message = badRequestMessage ?? "bad request" };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new T() { Success = false, Message = "no content" };

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var errorBody = await TryReadResponse<T>(response, cancellationToken);
                if (errorBody != null) return errorBody;
                return new T() { Success = false, Message = "internal server error" };
            }

            await response.EnsureSuccess();  
            var content = await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);            
            return content ?? new T() { Success = true, Message = "empty body" };
        }

        protected async Task Request(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await response.EnsureSuccess();
        }

        private async Task<T?> TryReadResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken) where T : Response, new()
        {
            // Read the body as string first (stream can only be consumed once)
            string? text = null;
            try
            {
                #if NET5_0_OR_GREATER
                    text = await response.Content.ReadAsStringAsync(cancellationToken);
                #else
                    text = await response.Content.ReadAsStringAsync();
                #endif
            }
            catch (Exception ex)
            {
                return new T() { Success = false, Message = NormalizeResponseMessage(text), Exception = ex };
            }

            if (string.IsNullOrWhiteSpace(text))
                return null;

            // Remove leading/trailing whitespace to improve chances of successful JSON parsing
            text = text.Trim();

            // Try to parse as JSON
            try
            {
                var body = System.Text.Json.JsonSerializer.Deserialize<T>(text, jsonOptions);
                if (body != null)
                {
                    body.Message = NormalizeResponseMessage(string.IsNullOrWhiteSpace(body.Message) ? text : body.Message);
                    return body;
                }
            }
            catch (Exception ex)
            {
                return new T() { Success = false, Message = NormalizeResponseMessage(text), Exception = ex };
            }

            return new T() { Success = false, Message = NormalizeResponseMessage(text) };
        }

        private static string? NormalizeResponseMessage(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var trimmed = text.Trim();
            if (LooksLikeHtml(trimmed))
            {
                // HTML here means the upstream API returned a rendered error page instead of the documented contract.
                // Prefer notifying the user/maintainer to align with the ReceitaNet programmer rather than adding
                // more HTML-specific workarounds here.
                return trimmed;
            }

            return trimmed;
        }

        private static bool LooksLikeHtml(string text)
            => text.IndexOf("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) >= 0
            || text.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0
            || text.IndexOf("<body", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
