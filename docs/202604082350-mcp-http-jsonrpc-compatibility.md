# ReceitaNet MCP HTTP JSON-RPC Compatibility

## About

This document records the local fix applied to the dedicated ReceitaNet MCP surface so VS Code MCP clients configured with `type: "http"` can initialize against the root path `/gateway/receitanet/mcp`.

## Problem

The published route already exposed discovery with `GET /gateway/receitanet/mcp` and legacy tool invocation with `POST /gateway/receitanet/mcp/tools/{toolName}`.
However, VS Code MCP HTTP clients send JSON-RPC messages to `POST /gateway/receitanet/mcp`.
Because the route returned `405 Method Not Allowed`, the client failed during `initialize` and then attempted an unnecessary fallback to legacy SSE.

## Changes

The dedicated package `sufficit-gateway-receitanet` now accepts JSON-RPC requests on the root MCP path without replacing the existing legacy helper endpoints.

Implemented changes:

- `EndpointRouteBuilderExtensions.MCP.cs`
  - Added `POST` root handler for JSON-RPC MCP methods.
  - Supports `initialize`, `notifications/initialized`, `tools/list`, `tools/call`, `logging/setLevel`, `prompts/list`, and `ping`.
  - Preserves `GET /tools` and `POST /tools/{toolName}` for backward compatibility.
- `ReceitaNetMcpSessionManager.cs`
  - Added lightweight in-memory session tracking.
  - Issues and validates `mcp-session-id` headers.
  - Renews session expiration on valid requests.
- `ReceitaNetMcpOptions.cs`
  - Added `SessionTtlMinutes` so the session lifetime can be configured without changing code.
- `ServiceCollectionExtensions.MCP.cs`
  - Registered the new session manager in DI.
  - Now guarantees the base ReceitaNet gateway client graph by calling `AddGatewayReceitaNet()` before wiring MCP-only services.

## Validation

Local validation completed with the following commands:

```powershell
dotnet build src/Sufficit.Gateway.ReceitaNet.csproj -c Release
dotnet test test/Sufficit.Gateway.ReceitaNet.IntegrationTests.csproj -c Release
dotnet build ../sufficit-endpoints/src/Sufficit.EndPoints.csproj -c Release
```

Results:

- `Sufficit.Gateway.ReceitaNet` built successfully for `netstandard2.0`, `net7.0`, and `net9.0`.
- `Sufficit.Gateway.ReceitaNet.IntegrationTests` passed with `16/16` tests.
- `Sufficit.EndPoints` built successfully with only pre-existing warnings.

Additional local runtime validation completed on `https://localhost:26503/gateway/receitanet/mcp`:

- `initialize` returned `200` and emitted `mcp-session-id`.
- `tools/list` returned `200` using the session header issued by `initialize`.
- `tools/call` returned `200` for the read-only tool `receitanet_contract_by_document`, proving root JSON-RPC request handling, session validation, token resolution, and tool invocation end-to-end.

## Operational note

This recorte adds the minimal MCP HTTP JSON-RPC contract required by the current VS Code client path.
It does not introduce a dedicated SSE transport for ReceitaNet.
The expected improvement is that the client no longer receives `405` on root `POST` during `initialize`.