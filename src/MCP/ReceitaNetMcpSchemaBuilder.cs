#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    internal static class ReceitaNetMcpSchemaBuilder
    {
        private static readonly NullabilityInfoContext NullabilityContext = new();

        public static ReceitaNetMcpToolInputSchema Build(Type inputType)
        {
            var schema = new ReceitaNetMcpToolInputSchema();

            foreach (var property in inputType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var propertySchema = new ReceitaNetMcpToolProperty()
                {
                    Type = GetSchemaType(propertyType),
                    Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name,
                    EnumValues = propertyType.IsEnum ? Enum.GetNames(propertyType) : null,
                };

                var propertyName = Json.Options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
                schema.Properties[propertyName] = propertySchema;

                if (IsRequired(property))
                    schema.Required.Add(propertyName);
            }

            return schema;
        }

        private static bool IsRequired(PropertyInfo property)
        {
            if (property.PropertyType.IsValueType)
                return Nullable.GetUnderlyingType(property.PropertyType) == null;

            var nullability = NullabilityContext.Create(property);
            return nullability.WriteState == NullabilityState.NotNull;
        }

        private static string GetSchemaType(Type propertyType)
        {
            if (propertyType == typeof(bool))
                return "boolean";

            if (propertyType == typeof(byte) || propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(uint) || propertyType == typeof(ushort) || propertyType == typeof(ulong))
                return "integer";

            if (propertyType == typeof(float) || propertyType == typeof(double) || propertyType == typeof(decimal))
                return "number";

            if (propertyType.IsEnum)
                return "string";

            return "string";
        }
    }
}
#endif