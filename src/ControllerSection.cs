using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Gateway.ReceitaNet.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<T>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>(jsonOptions, cancellationToken) ?? Array.Empty<T>();
        }

        protected async Task<T> Request<T>(HttpRequestMessage message, CancellationToken cancellationToken) where T : Response, new()
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new T() { Success = false, Message = "not found" };

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new T() { Success = false, Message = "no content" };

            await response.EnsureSuccess();  
            var content = await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);            
            return content ?? new T() { Success = true, Message = "empty body" };
        }

        protected async Task Request(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess();
        }
    }
}
