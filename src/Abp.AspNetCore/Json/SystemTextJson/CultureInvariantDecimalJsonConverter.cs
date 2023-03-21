using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson
{
    public class CultureInvariantDecimalJsonConverter : JsonConverter<decimal>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
                {
                    return result;
                }
            }

            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            _writeJsonSerializerOptions ??= JsonSerializerOptionsHelper.Create(options, this);
            var converter = (JsonConverter<decimal?>)_writeJsonSerializerOptions.GetConverter(typeof(decimal?));
            converter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }

    public class CultureInvariantNullableDecimalJsonConverter : JsonConverter<decimal?>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
                {
                    return result;
                }
            }

            if (reader.TryGetDecimal(out var d))
            {
                return d;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            _writeJsonSerializerOptions ??= JsonSerializerOptionsHelper.Create(options, this);
            var converter = (JsonConverter<decimal?>)_writeJsonSerializerOptions.GetConverter(typeof(decimal?));
            converter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }
}
