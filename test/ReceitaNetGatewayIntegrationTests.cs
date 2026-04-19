using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

[Trait("Category", "Integration")]
public class ReceitaNetGatewayIntegrationTests
{
    private readonly ITestOutputHelper _output;
    private readonly IntegrationTestSettings _settings;
    private readonly APIClientService _client;

    public ReceitaNetGatewayIntegrationTests(ITestOutputHelper output)
    {
        _output = output;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _settings = configuration.GetSection("ReceitaNet").Get<IntegrationTestSettings>() ?? new IntegrationTestSettings();
        _settings.BaseUrl = string.IsNullOrWhiteSpace(_settings.BaseUrl)
            ? "https://sistema.receitanet.net/api/novo/ura/"
            : _settings.BaseUrl;

        _client = ReceitaNetTestClientFactory.Create(
            new StaticHttpClientFactory(() => new HttpClient()),
            new GatewayOptions()
            {
                BaseUrl = _settings.BaseUrl,
                ClientId = "ReceitaNetIntegrationTests",
                Agent = "Sufficit Gateway ReceitaNet Integration Tests",
                TimeOut = 30
            });
    }

    [Fact]
    public async Task GetContractByDocument_ReturnsContracts_WhenConfigured()
    {
        if (!_settings.HasDocumentLookup)
        {
            _output.WriteLine("Skipping document lookup integration test: missing ReceitaNet:Token or ReceitaNet:Document.");
            return;
        }

        var response = await _client.GetContractByDocument(_settings.Token!, _settings.Document!);

        if (!response.Success || response.Contracts.Count == 0)
        {
            _output.WriteLine($"Skipping document lookup assertion: provider returned success={response.Success}, message={response.Message}.");
            return;
        }

        Assert.NotNull(response);
        Assert.True(response.Success, response.Message);
        Assert.NotEmpty(response.Contracts);

        _output.WriteLine($"Document lookup returned {response.Contracts.Count} contract(s).");
    }

    [Fact]
    public async Task GetContractByPhone_ReturnsContracts_WhenConfigured()
    {
        if (!_settings.HasPhoneLookup)
        {
            _output.WriteLine("Skipping phone lookup integration test: missing ReceitaNet:Token or ReceitaNet:Phone.");
            return;
        }

        var response = await _client.GetContractByPhone(_settings.Token!, _settings.Phone!);

        if (!response.Success || response.Contracts.Count == 0)
        {
            _output.WriteLine($"Skipping phone lookup assertion: provider returned success={response.Success}, message={response.Message}.");
            return;
        }

        Assert.NotNull(response);
        Assert.True(response.Success, response.Message);
        Assert.NotEmpty(response.Contracts);

        _output.WriteLine($"Phone lookup returned {response.Contracts.Count} contract(s).");
    }

    [Fact]
    public async Task GetConnectionStatus_ReturnsResponse_WhenContractAndPhoneAreConfigured()
    {
        var contractId = await ResolveContractIdAsync();
        if (!contractId.HasValue || string.IsNullOrWhiteSpace(_settings.Phone) || !_settings.HasToken)
        {
            _output.WriteLine("Skipping connection status integration test: contract, phone, or token not configured.");
            return;
        }

        var response = await _client.GetConnectionStatus(new Parameters.ContractAndContactParameters()
        {
            ContractId = contractId.Value,
            Contact = _settings.Phone
        }, _settings.Token!);

        Assert.NotNull(response);
        Assert.True(IsControlledConnectionStatus(response), response.Message);
        _output.WriteLine($"Connection status response: success={response.Success}, status={response.Status}, message={response.Message}.");
    }

    [Fact]
    public async Task ChargeNotification_RunsWhenSideEffectsAreEnabled()
    {
        var contractId = await ResolveContractIdAsync();
        if (!ShouldRunMutatingTest(contractId))
            return;

        var response = await _client.ChargeNotification(new Parameters.ChargeNotificationParameters()
        {
            ContractId = contractId!.Value,
            Contact = _settings.Phone,
            Channel = NotificationChannel.email
        }, _settings.Token!);

        Assert.NotNull(response);
        Assert.True(IsControlledChargeNotification(response), response.Message);
        _output.WriteLine($"Charge notification response: success={response.Success}, status={response.Status}, message={response.Message}, protocol={response.Protocol}.");
    }

    [Fact]
    public async Task PaymentNotification_RunsWhenSideEffectsAreEnabled()
    {
        var contractId = await ResolveContractIdAsync();
        if (!ShouldRunMutatingTest(contractId))
            return;

        var response = await _client.PaymentNotification(new Parameters.ContractAndContactParameters()
        {
            ContractId = contractId!.Value,
            Contact = _settings.Phone
        }, _settings.Token!);

        Assert.NotNull(response);
        Assert.True(response.Success, response.Message);
        _output.WriteLine($"Payment notification status: {response.Status}, allowed={response.Allowed}.");
    }

    [Fact]
    public async Task TicketAndRecording_RunWhenSideEffectsAreEnabled()
    {
        var contractId = await ResolveContractIdAsync();
        if (!ShouldRunMutatingTest(contractId))
            return;

        var ticketId = _settings.TicketId;
        if (!ticketId.HasValue)
        {
            var ticketResponse = await _client.Ticket(new Parameters.TicketParameters()
            {
                ContractId = contractId!.Value,
                Contact = _settings.Phone,
                Kind = _settings.TicketKind,
                Reason = _settings.TicketReason
            }, _settings.Token!);

            Assert.True(ticketResponse.Success, ticketResponse.Message);
            Assert.True(ticketResponse.Status);
            ticketId = ticketResponse.TicketId;
            _output.WriteLine($"Ticket created for integration test: {ticketId}.");
        }

        var recordingResponse = await _client.Recording(new Parameters.RecordingParameters()
        {
            TicketId = ticketId.Value,
            Recording = _settings.RecordingUrl,
            Extension = _settings.RecordingExtension,
            Finalize = _settings.RecordingFinalize
        }, _settings.Token!);

        Assert.NotNull(recordingResponse);
        Assert.True(recordingResponse.Success, recordingResponse.Message);
        Assert.True(recordingResponse.Status);
        _output.WriteLine($"Recording update accepted for ticket {ticketId}.");
    }

    private bool ShouldRunMutatingTest(int? contractId)
    {
        if (!_settings.AllowSideEffects)
        {
            _output.WriteLine("Skipping mutating integration test: ReceitaNet:AllowSideEffects is false.");
            return false;
        }

        if (!contractId.HasValue || string.IsNullOrWhiteSpace(_settings.Phone) || !_settings.HasToken)
        {
            _output.WriteLine("Skipping mutating integration test: contract, phone, or token not configured.");
            return false;
        }

        return true;
    }

    private static bool IsControlledConnectionStatus(Responses.ConnectionStatusResponse response)
        => response.Success
        || response.Status == Responses.ConnectionStatusResponse.ConnectionStatus.Connected
        || response.Status == Responses.ConnectionStatusResponse.ConnectionStatus.Disconnected;

    private static bool IsControlledChargeNotification(Responses.ChargeResponse response)
        => response.Success
        || response.Message?.Contains("boleto pendente", StringComparison.OrdinalIgnoreCase) == true
        || response.Message?.Contains("nenhum boleto", StringComparison.OrdinalIgnoreCase) == true;

    private async Task<int?> ResolveContractIdAsync()
    {
        if (_settings.ContractId.HasValue)
            return _settings.ContractId;

        if (_settings.HasDocumentLookup)
        {
            var response = await _client.GetContractByDocument(_settings.Token!, _settings.Document!);
            if (response.Success && response.Contracts.Count > 0)
                return response.Contracts[0].EffectiveContractId;
        }

        if (_settings.HasPhoneLookup)
        {
            var response = await _client.GetContractByPhone(_settings.Token!, _settings.Phone!);
            if (response.Success && response.Contracts.Count > 0)
                return response.Contracts[0].EffectiveContractId;
        }

        return null;
    }
}