namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

public class ContractResponseJsonConverterTests
{
    [Fact]
    public void Deserialize_SingleContractPayload_ExposesPrimaryContract()
    {
        var response = System.Text.Json.JsonSerializer.Deserialize<Responses.ContractResponse>(SamplePayloads.ContractSingle, Json.Options);

        Assert.NotNull(response);
        Assert.True(response!.Success);
        Assert.Single(response.Contracts);
        Assert.False(response.HasMultipleContracts);
        Assert.NotNull(response.Contract);
        Assert.Equal(123, response.Contract!.ClientId);
    }

    [Fact]
    public void Deserialize_MultiContractPayload_ExposesAllContractsAndFirstShortcut()
    {
        var response = System.Text.Json.JsonSerializer.Deserialize<Responses.ContractResponse>(SamplePayloads.ContractMany, Json.Options);

        Assert.NotNull(response);
        Assert.True(response!.Success);
        Assert.Equal(2, response.Contracts.Count);
        Assert.True(response.HasMultipleContracts);
        Assert.NotNull(response.Contract);
        Assert.Equal(123, response.Contract!.ClientId);
        Assert.Equal(456, response.Contracts[1].ClientId);
    }
}