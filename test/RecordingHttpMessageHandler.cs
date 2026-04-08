using System.Net;
using System.Net.Http;
using System.Text;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

    public RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public List<RecordedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content == null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        Requests.Add(new RecordedRequest(request.Method, request.RequestUri?.ToString(), body));
        return _responseFactory(request);
    }

    public static HttpResponseMessage Json(string content)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
}

internal sealed record RecordedRequest(HttpMethod Method, string? RequestUri, string? Body);