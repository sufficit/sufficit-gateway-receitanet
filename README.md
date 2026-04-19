<p align="center">
	<img src="https://raw.githubusercontent.com/sufficit/sufficit-gateway-receitanet/main/icon.png" alt="ReceitaNet" width="100" />
</p>

# Sufficit.Gateway.ReceitaNet

## About

`Sufficit.Gateway.ReceitaNet` is a .NET client library for the official ReceitaNet URA API.
It wraps the main provider operations used by Sufficit integrations, including customer lookup, connection status checks, billing notifications, payment notifications, ticket creation, and recording updates.

The package keeps the ReceitaNet token explicit per request, because the external API expects the token in the query string instead of using Bearer authentication.

## Features

- Customer lookup by contract, phone, or CPF/CNPJ
- Connection status queries
- Billing notification by email or SMS
- Payment notification requests
- Ticket creation for support flows
- Ticket recording update requests
- Support for `contratos` returned as either a single object or an array
- Request contract tests and optional live integration tests

## Installation

Add the NuGet package reference:

```xml
<PackageReference Include="Sufficit.Gateway.ReceitaNet" Version="*" />
```

Register the gateway in dependency injection:

```csharp
services.AddGatewayReceitaNet();
```

Configure the `ReceitaNet` section in your application settings:

```json
{
	"ReceitaNet": {
		"BaseUrl": "https://sistema.receitanet.net/api/novo/ura/",
		"ClientId": "ReceitaNet",
		"TimeOut": 30,
		"Agent": "Sufficit C# API Client"
	}
}
```

## Usage

Inject `APIClientService` and pass the provider token explicitly on each request:

```csharp
public class ReceitaNetProbe
{
		private readonly APIClientService _client;

		public ReceitaNetProbe(APIClientService client)
		{
				_client = client;
		}

		public async Task<int?> FindContractByDocumentAsync(string token, string document, CancellationToken cancellationToken)
		{
				var response = await _client.GetContractByDocument(token, document, cancellationToken);
				return response.Contract?.EffectiveContractId;
		}
}
```

The official ReceitaNet payload currently exposes both `idCliente` and `contratoId` in lookup responses.
Use `ContractId` or `EffectiveContractId` for follow-up contract-scoped operations such as status, notifications, and ticket creation.
`EffectiveContractId` falls back to `ClientId` only to preserve compatibility with older payloads that omit `contratoId`.

Main operations exposed by the package:

- `GetContract(...)`
- `GetContractByPhone(...)`
- `GetContractByDocument(...)`
- `GetConnectionStatus(...)`
- `ChargeNotification(...)`
- `PaymentNotification(...)`
- `Ticket(...)`
- `Recording(...)`

## Tests

Tests live under `test/`.

Run the full suite with:

```bash
dotnet test test/Sufficit.Gateway.ReceitaNet.IntegrationTests.csproj -c Release
```

The suite includes:

- HTTP request contract tests for each ReceitaNet endpoint
- `ContractResponse` deserialization tests for single-object and array payloads
- Guardrail tests for `ProtectedApiQueryTokenHandler`
- Optional live integration tests driven by `test/appsettings.json` or environment variables

Mutating tests stay disabled by default through `ReceitaNet:AllowSideEffects = false`.
Only enable them when you explicitly want to execute real notification, ticket, or recording operations.

## License

This repository is distributed under the license shipped in [license](license).

## Support

- Issues: https://github.com/sufficit/sufficit-gateway-receitanet/issues
- Telegram: https://t.me/sufficitti/2

## Related

- Official ReceitaNet API documentation: https://www.receitanet.net/api/ura/
- Official ReceitaNet OpenAPI: https://www.receitanet.net/api/ura/openapi.yaml
- Repository: https://github.com/sufficit/sufficit-gateway-receitanet


