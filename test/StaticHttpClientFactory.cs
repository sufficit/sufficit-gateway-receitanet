using System.Net.Http;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal sealed class StaticHttpClientFactory : IHttpClientFactory
{
    private readonly Func<HttpClient> _clientFactory;

    public StaticHttpClientFactory(Func<HttpClient> clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public HttpClient CreateClient(string name) => _clientFactory();
}