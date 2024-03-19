using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sufficit.Gateway.ReceitaNet
{
    public static class Json
    {
        /// <summary>
        /// Use default json options
        /// </summary>
        public static JsonSerializerOptions Options { get; } = Generate();

        /// <summary>
        /// If you need an unmodified version
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions Generate()
        {
            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                AllowTrailingCommas = true,
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
            options.Converters.Add(new JsonStringTypeConverter());
            return options;
        }

        public const string DATETIMEFORMAT = "yyyy-MM-dd";    
    }

    public class JsonStringTypeConverter : JsonConverter<Type>
    {
        public override Type? Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __)
            => Type.GetType(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, Type type, JsonSerializerOptions _)
            => writer.WriteStringValue(type.ToString());
    }

    public class DateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.ParseExact(reader.GetString()!, Json.DATETIMEFORMAT, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString(Json.DATETIMEFORMAT));
        }
    }

    public class BoolToStringConverter : JsonConverter<string?>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(bool) || typeToConvert == typeof(string);
        }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return null;                
                default:
                    return reader.GetString();
            }
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);            
        }
    }
}
