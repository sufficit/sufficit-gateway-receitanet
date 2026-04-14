using System.IO;
using System.Net;
using System.Net.Http;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal sealed class TrackingHttpContent : HttpContent
{
    public bool WasRead { get; private set; }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        WasRead = true;
        return Task.CompletedTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }
}