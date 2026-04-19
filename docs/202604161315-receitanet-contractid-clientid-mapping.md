# ReceitaNet ContractId And ClientId Mapping

## About

This note documents the identifier mapping used by ReceitaNet payloads and by the local gateway models.

## Official Contract

The official OpenAPI document declares both `idCliente` and `contratoId` in the `Cliente` schema and in contract-scoped responses such as connection status, payment notification, charge notification, and ticket creation.

## Runtime Validation

On 2026-04-16, a live compatibility verification against the official ReceitaNet endpoint confirmed that lookup responses from `/clientes` returned both `idCliente` and `contratoId` for the selected compatibility scenario.
The same live round also confirmed `contratoId` in contract-scoped responses.

## Mapping Rules

- `ClientId` maps the official `idCliente` field.
- `ContractId` maps the official `contratoId` field.
- `ContractId` must be preferred for contract-scoped follow-up operations.
- `EffectiveContractId` is the safe helper for callers that need one value: it prefers `ContractId` and falls back to `ClientId` only for backward compatibility with older payloads that omit `contratoId`.

## Consumer Guidance

- Do not assume `ClientId` replaced `ContractId`.
- Do not mark `ContractId` as obsolete while the upstream contract still documents and returns it.
- Compatibility flows, tests, and legacy FastAGI consumers should use `EffectiveContractId` when they need a single contract reference.