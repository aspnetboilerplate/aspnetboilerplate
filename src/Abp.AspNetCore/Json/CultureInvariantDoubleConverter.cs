using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Abp.Json;

public class CultureInvariantDoubleConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            if (reader.Value == null || (string)reader.Value == string.Empty)
            {
                return null;
            }
        }

        if (reader.Value == null)
        {
            return null;
        }

        if (double.TryParse(reader.Value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
        {
            return result;
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(double) || objectType == typeof(double?);
    }
}