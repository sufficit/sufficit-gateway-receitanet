using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Nearly the HttpResponseMessage.EnsureSuccessStatusCode(), but reads the content from request before throws
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async ValueTask EnsureSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
#if NET5_0_OR_GREATER
                throw new HttpRequestException(content, new Exception(response.ReasonPhrase), response.StatusCode);
#else
                throw new HttpRequestException(content);
#endif
            }
        }

        public static HttpClient Configure(this IHttpClientFactory factory, GatewayOptions options)
            => factory.CreateClient(options.ClientId).Configure(options);

        public static HttpClient Configure(this HttpClient source, GatewayOptions options)
        {
            source.BaseAddress = new Uri(options.BaseUrl);

            if (options.TimeOut.HasValue)
                source.Timeout = TimeSpan.FromSeconds(options.TimeOut.Value);

            source.DefaultRequestHeaders.Add("User-Agent", options.Agent);
            return source;
        }
    }
}
