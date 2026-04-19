namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal static class SamplePayloads
{
    public const string ContractSingle = """
    {
      "success": true,
      "msg": "ok",
      "contratos": {
        "idCliente": 123,
        "contratoId": 123,
        "razaoSocial": "Cliente Teste",
        "contratoStatusDisplay": "Ativo",
        "isPromessaPagamento": false,
        "contratoValorAberto": 0,
        "faturasEmAberto": [],
        "cpfCnpj": "12345678901",
        "contratoStatus": 1,
        "existeChamado": 0,
        "isChamados": { "1": 0, "2": 0, "4": 0, "5": 0 },
        "isEmail": true,
        "isSMS": true,
        "tecnologia": 1,
        "servidor": { "isManutencao": false, "mensagemManutencao": false }
      }
    }
    """;

    public const string ContractSingleLegacyWithoutContractId = """
    {
      "success": true,
      "msg": "ok",
      "contratos": {
        "idCliente": 123,
        "razaoSocial": "Cliente Teste",
        "contratoStatusDisplay": "Ativo",
        "isPromessaPagamento": false,
        "contratoValorAberto": 0,
        "faturasEmAberto": [],
        "cpfCnpj": "12345678901",
        "contratoStatus": 1,
        "existeChamado": 0,
        "isChamados": { "1": 0, "2": 0, "4": 0, "5": 0 },
        "isEmail": true,
        "isSMS": true,
        "tecnologia": 1,
        "servidor": { "isManutencao": false, "mensagemManutencao": false }
      }
    }
    """;

    public const string ContractMany = """
    {
      "success": true,
      "msg": "ok",
      "contratos": [
        {
          "idCliente": 123,
          "contratoId": 123,
          "razaoSocial": "Cliente A",
          "contratoStatusDisplay": "Ativo",
          "isPromessaPagamento": false,
          "contratoValorAberto": 10,
          "faturasEmAberto": [],
          "cpfCnpj": "12345678901",
          "contratoStatus": 1,
          "existeChamado": 0,
          "isChamados": { "1": 0, "2": 0, "4": 0, "5": 0 },
          "isEmail": true,
          "isSMS": true,
          "servidor": { "isManutencao": false, "mensagemManutencao": false }
        },
        {
          "idCliente": 456,
          "contratoId": 456,
          "razaoSocial": "Cliente B",
          "contratoStatusDisplay": "Suspenso",
          "isPromessaPagamento": true,
          "contratoValorAberto": 20,
          "faturasEmAberto": [],
          "cpfCnpj": "12345678901",
          "contratoStatus": 3,
          "existeChamado": 1,
          "isChamados": { "1": 1, "2": 0, "4": 0, "5": 0 },
          "isEmail": true,
          "isSMS": false,
          "servidor": { "isManutencao": true, "mensagemManutencao": "Maintenance" }
        }
      ]
    }
    """;

    public const string ConnectionStatus = """
    {
      "success": true,
      "msg": "ok",
      "idCliente": 123,
      "contratoId": 123,
      "cpfCnpj": "12345678901",
      "razaoSocial": "Cliente Teste",
      "uracontato": "21999999999",
      "status": 1
    }
    """;

    public const string NotifyAllowed = """
    {
      "success": true,
      "msg": "ok",
      "idCliente": 123,
      "contratoId": 123,
      "cpfCnpj": "12345678901",
      "razaoSocial": "Cliente Teste",
      "uracontato": "21999999999",
      "protocolo": "20260408101010",
      "status": 1,
      "liberado": true
    }
    """;

    public const string Charge = """
    {
      "success": true,
      "msg": "ok",
      "idCliente": 123,
      "contratoId": 123,
      "cpfCnpj": "12345678901",
      "razaoSocial": "Cliente Teste",
      "uracontato": "21999999999",
      "protocolo": "20260408101010",
      "status": true
    }
    """;

    public const string ChargeNotFound = """
    {
      "success": false,
      "msg": "Nenhum boleto pendente localizado"
    }
    """;

    public const string Ticket = """
    {
      "success": true,
      "msg": "ok",
      "idCliente": 123,
      "contratoId": 123,
      "cpfCnpj": "12345678901",
      "razaoSocial": "Cliente Teste",
      "uracontato": "21999999999",
      "protocolo": "20260408101010",
      "status": true,
      "idSuporte": 9876
    }
    """;

    public const string Recording = """
    {
      "success": true,
      "msg": "ok",
      "idCliente": 123,
      "status": true
    }
    """;
}