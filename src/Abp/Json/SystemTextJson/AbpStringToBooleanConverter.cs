using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson
{
    public class AbpStringToBooleanConverter : JsonConverter<bool>
    {
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out bool b1, out var bytesConsumed) && span.Length == bytesConsumed)
                {
                    return b1;
                }

                if (bool.TryParse(reader.GetString(), out var b2))
                {
                    return b2;
                }
            }

            return reader.GetBoolean();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (_writeJsonSerializerOptions == null)
            {
                _writeJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, this);
            }

            var entityConverter = (JsonConverter<bool>)_writeJsonSerializerOptions.GetConverter(typeof(bool));
            entityConverter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }
}
