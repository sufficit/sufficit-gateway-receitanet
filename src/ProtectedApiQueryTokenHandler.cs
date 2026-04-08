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
                var query = System.Web.HttpUtility.ParseQueryString(request.RequestUri?.Query ?? string.Empty);
                var token = query["token"];
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception("missing token");
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
