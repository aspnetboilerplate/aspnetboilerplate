using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson
{
    public class AbpStringToGuidConverter : JsonConverter<Guid>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var guidString = reader.GetString();
                string[] formats = { "N", "D", "B", "P", "X" };
                foreach (var format in formats)
                {
                    if (Guid.TryParseExact(guidString, format, out var guid))
                    {
                        return guid;
                    }
                }
            }

            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            if (_writeJsonSerializerOptions == null)
            {
                _writeJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, this);
            }

            var entityConverter = (JsonConverter<Guid>)_writeJsonSerializerOptions.GetConverter(typeof(Guid));
            entityConverter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }

    public class AbpNullableStringToGuidConverter : JsonConverter<Guid?>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var guidString = reader.GetString();
                string[] formats = { "N", "D", "B", "P", "X" };
                foreach (var format in formats)
                {
                    if (Guid.TryParseExact(guidString, format, out var guid))
                    {
                        return guid;
                    }
                }
            }

            if (reader.TryGetGuid(out var guid2))
            {
                return guid2;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            if (_writeJsonSerializerOptions == null)
            {
                _writeJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, this);
            }

            var entityConverter = (JsonConverter<Guid?>)_writeJsonSerializerOptions.GetConverter(typeof(Guid?));
            entityConverter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }

}
