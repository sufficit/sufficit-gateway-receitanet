using System.Net;
using System.Text;
using System.Text.Json;
using Sufficit.Gateway.ReceitaNet.Parameters;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

public class APIClientServiceRequestTests
{
    [Fact]
    public async Task GetContract_SendsTokenInQueryAndAppOnlyInBody()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.ContractSingle));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.GetContractByDocument("token-123", "12345678901");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("https://example.test/api/novo/ura/clientes?token=token-123", request.RequestUri);
        Assert.DoesNotContain("app=", request.RequestUri, StringComparison.OrdinalIgnoreCase);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal("12345678901", body.RootElement.GetProperty("cpfcnpj").GetString());
        Assert.False(body.RootElement.TryGetProperty("contrato", out _));
    }

    [Fact]
    public async Task GetConnectionStatus_SendsRequiredFieldsInBody()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.ConnectionStatus));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.GetConnectionStatus(new ContractAndContactParameters()
        {
            ContractId = 321,
            Contact = "21999999999"
        }, "token-123");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal("https://example.test/api/novo/ura/verificar-acesso?token=token-123", request.RequestUri);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal(321, body.RootElement.GetProperty("contrato").GetInt32());
        Assert.Equal("21999999999", body.RootElement.GetProperty("uracontato").GetString());
    }

    [Fact]
    public async Task GetConnectionStatus_ReturnsContractNotFoundWithoutReadingBodyOn404()
    {
        var content = new TrackingHttpContent();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = content
        });
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.GetConnectionStatus(new ContractAndContactParameters()
        {
            ContractId = 321,
            Contact = "21999999999"
        }, "token-123");

        Assert.False(response.Success);
        Assert.Equal("contract not found", response.Message);
        Assert.NotNull(response.Exception);
        Assert.Equal("contract not found", response.Exception!.Message);
        Assert.False(content.WasRead);
    }

    [Fact]
    public async Task GetConnectionStatus_PreservesHtmlErrorPageWhenUpstreamReturnsHtml()
    {
        const string html = """
<!DOCTYPE html>
<html>
<body>
    <div class=\"error-page\">
        <h2 class=\"headline text-yellow\">500</h2>
        <div class=\"error-content\">
            <h3><i class=\"fa fa-warning text-yellow\"></i> Oops! Erro Interno do Servidor</h3>
            <p>O servidor encontrou uma condição inesperada.</p>
        </div>
    </div>
</body>
</html>
""";

        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(html, Encoding.UTF8, "text/html")
        });
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.GetConnectionStatus(new ContractAndContactParameters()
        {
            ContractId = 321,
            Contact = "21999999999"
        }, "token-123");

        Assert.False(response.Success);
        Assert.Equal(html.Trim(), response.Message);
        Assert.NotNull(response.Exception);
        Assert.Equal(response.Message, response.Exception!.Message);
        Assert.DoesNotContain("error parsing response", response.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<html", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ChargeNotification_SendsApplicationAndChannelInBody()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.Charge));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.ChargeNotification(new ChargeNotificationParameters()
        {
            ContractId = 321,
            Contact = "21999999999",
            Channel = NotificationChannel.email
        }, "token-123");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal("https://example.test/api/novo/ura/boletos?token=token-123", request.RequestUri);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal("email", body.RootElement.GetProperty("tipo").GetString());
        Assert.Equal(321, body.RootElement.GetProperty("contrato").GetInt32());
        Assert.Equal("21999999999", body.RootElement.GetProperty("uracontato").GetString());
    }

    [Fact]
    public async Task ChargeNotification_ReturnsStructuredErrorWhenUpstreamReturnsBadRequest()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(SamplePayloads.ChargeNotFound, Encoding.UTF8, "application/json")
        });
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.ChargeNotification(new ChargeNotificationParameters()
        {
            ContractId = 321,
            Contact = "21999999999",
            Channel = NotificationChannel.email
        }, "token-123");

        Assert.False(response.Success);
        Assert.False(response.Status);
        Assert.Equal("Nenhum boleto pendente localizado", response.Message);
        Assert.Null(response.Exception);
    }

    [Fact]
    public async Task PaymentNotification_SendsApplicationAndDoesNotUseQueryApp()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.NotifyAllowed));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.PaymentNotification(new ContractAndContactParameters()
        {
            ContractId = 321,
            Contact = "21999999999"
        }, "token-123");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal("https://example.test/api/novo/ura/notificacao-pagamento?token=token-123", request.RequestUri);
        Assert.DoesNotContain("app=", request.RequestUri, StringComparison.OrdinalIgnoreCase);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal(321, body.RootElement.GetProperty("contrato").GetInt32());
        Assert.Equal("21999999999", body.RootElement.GetProperty("uracontato").GetString());
    }

    [Fact]
    public async Task Ticket_SendsApplicationAndBusinessFieldsInBody()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.Ticket));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.Ticket(new TicketParameters()
        {
            ContractId = 321,
            Contact = "21999999999",
            Kind = TicketEnum.Maintenance,
            Reason = "Integration test"
        }, "token-123");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal("https://example.test/api/novo/ura/abertura-chamado?token=token-123", request.RequestUri);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal(321, body.RootElement.GetProperty("contrato").GetInt32());
        Assert.Equal("21999999999", body.RootElement.GetProperty("uracontato").GetString());
        Assert.Equal("Integration test", body.RootElement.GetProperty("motivoos").GetString());
        Assert.Equal((int)TicketEnum.Maintenance, body.RootElement.GetProperty("ocorrenciatipo").GetInt32());
    }

    [Fact]
    public async Task Recording_SendsApplicationAndExplicitPayloadFieldsInBody()
    {
        var handler = new RecordingHttpMessageHandler(_ => RecordingHttpMessageHandler.Json(SamplePayloads.Recording));
        var client = ReceitaNetTestClientFactory.Create(new StaticHttpClientFactory(() => new HttpClient(handler, false)));

        var response = await client.Recording(new RecordingParameters()
        {
            TicketId = 9876,
            Recording = "https://example.test/test.wav",
            Extension = 1000,
            Finalize = false
        }, "token-123");

        Assert.True(response.Success);
        var request = Assert.Single(handler.Requests);
        Assert.Equal("https://example.test/api/novo/ura/chamado-gravacao?token=token-123", request.RequestUri);

        using var body = JsonDocument.Parse(request.Body!);
        Assert.Equal(APIClientService.APPLICATION, body.RootElement.GetProperty("app").GetString());
        Assert.Equal(9876, body.RootElement.GetProperty("idSuporte").GetInt32());
        Assert.Equal("https://example.test/test.wav", body.RootElement.GetProperty("urlgravacao").GetString());
        Assert.Equal(1000, body.RootElement.GetProperty("ramal").GetInt32());
        Assert.False(body.RootElement.GetProperty("is_finalizar").GetBoolean());
    }
}