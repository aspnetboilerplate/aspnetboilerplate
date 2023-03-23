using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to#deserialize-inferred-types-to-object-properties
    /// </summary>
    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                    break;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l))
                    {
                        return l;
                    }
                    return reader.GetDouble();
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var datetime))
                    {
                        return datetime;
                    }
                    return reader.GetString();
            }

            return JsonDocument.ParseValue(ref reader).RootElement.Clone();
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
        }
    }
}
