using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson
{
    public class CultureInvariantDoubleJsonConverter : JsonConverter<double>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
                {
                    return result;
                }
            }

            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            _writeJsonSerializerOptions ??= JsonSerializerOptionsHelper.Create(options, this);
            var converter = (JsonConverter<double?>)_writeJsonSerializerOptions.GetConverter(typeof(double?));
            converter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }

    public class CultureInvariantNullableDoubleJsonConverter : JsonConverter<double?>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
                {
                    return result;
                }
            }

            if (reader.TryGetDouble(out var d))
            {
                return d;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            _writeJsonSerializerOptions ??= JsonSerializerOptionsHelper.Create(options, this);
            var converter = (JsonConverter<double?>)_writeJsonSerializerOptions.GetConverter(typeof(double?));
            converter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }
}
