using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Gateway.ReceitaNet
{
    public class ProtectedApiQueryTokenHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ShouldAuthenticate(request)) 
            {
                if (!request.RequestUri?.Query.Contains("token") ?? true)
                    throw new Exception("missing token");

                if (!request.RequestUri?.Query.Contains("app") ?? true)
                    throw new Exception("missing app");
            }

            // Proceed calling the inner handler, that will actually send the request
            // to our protected api
            return await base.SendAsync(request, cancellationToken);
        }

        protected bool ShouldAuthenticate(HttpRequestMessage request)
        {
            switch (request.RequestUri?.AbsolutePath)
            {
                // case "/contact":
                default: return true;
            }
        }
    }
}
