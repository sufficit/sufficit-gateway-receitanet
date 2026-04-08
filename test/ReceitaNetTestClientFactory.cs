using Microsoft.Extensions.Logging.Abstractions;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal static class ReceitaNetTestClientFactory
{
    public static APIClientService Create(StaticHttpClientFactory factory, GatewayOptions? options = null)
    {
        options ??= new GatewayOptions()
        {
            BaseUrl = "https://example.test/api/novo/ura/",
            ClientId = "ReceitaNetTests",
            Agent = "Sufficit Gateway ReceitaNet Tests"
        };

        return new APIClientService(
            new FixedOptionsMonitor<GatewayOptions>(options),
            factory,
            NullLogger<APIClientService>.Instance);
    }
}