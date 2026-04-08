namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

public class ProtectedApiQueryTokenHandlerTests
{
    [Fact]
    public async Task SendAsync_AllowsRequest_WhenTokenQueryParameterIsPresent()
    {
        var handler = new ProtectedApiQueryTokenHandler()
        {
            InnerHandler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json("{\"success\":true,\"msg\":\"ok\"}"))
        };

        using var invoker = new HttpMessageInvoker(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api?token=abc123");

        using var response = await invoker.SendAsync(request, CancellationToken.None);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SendAsync_Throws_WhenTokenQueryParameterIsMissing()
    {
        var handler = new ProtectedApiQueryTokenHandler()
        {
            InnerHandler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json("{\"success\":true,\"msg\":\"ok\"}"))
        };

        using var invoker = new HttpMessageInvoker(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api?mytoken=abc123");

        var exception = await Assert.ThrowsAsync<Exception>(() => invoker.SendAsync(request, CancellationToken.None));

        Assert.Equal("missing token", exception.Message);
    }
}