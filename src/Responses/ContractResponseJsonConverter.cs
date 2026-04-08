using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet.Responses
{
    public sealed class ContractResponseJsonConverter : JsonConverter<ContractResponse>
    {
        public override ContractResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            var response = new ContractResponse();

            if (root.TryGetProperty("success", out var successElement) && successElement.ValueKind is JsonValueKind.True or JsonValueKind.False)
                response.Success = successElement.GetBoolean();

            if (root.TryGetProperty("msg", out var messageElement) && messageElement.ValueKind != JsonValueKind.Null)
                response.Message = messageElement.GetString();

            if (root.TryGetProperty("contratos", out var contractsElement))
                response.Contracts = ReadContracts(contractsElement, options);

            return response;
        }

        public override void Write(Utf8JsonWriter writer, ContractResponse value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("success", value.Success);

            if (value.Message != null)
                writer.WriteString("msg", value.Message);
            else
                writer.WriteNull("msg");

            writer.WritePropertyName("contratos");
            if (value.Contracts.Count == 1)
                JsonSerializer.Serialize(writer, value.Contracts[0], options);
            else
                JsonSerializer.Serialize(writer, value.Contracts, options);

            writer.WriteEndObject();
        }

        private static IReadOnlyList<Contract> ReadContracts(JsonElement element, JsonSerializerOptions options)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var contract = element.Deserialize<Contract>(options);
                        return contract == null ? Array.Empty<Contract>() : new[] { contract };
                    }
                case JsonValueKind.Array:
                    {
                        var contracts = element.Deserialize<List<Contract>>(options);
                        return contracts ?? (IReadOnlyList<Contract>)Array.Empty<Contract>();
                    }
                default:
                    return Array.Empty<Contract>();
            }
        }
    }
}