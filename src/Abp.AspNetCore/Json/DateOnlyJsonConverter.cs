using Newtonsoft.Json;
using System;

namespace Abp.Json;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("O"));
    }

    public override DateOnly ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null)
        {
            return DateOnly.MinValue;
        }

        var dateValueType = reader.Value.GetType();
        if (dateValueType == typeof(DateTimeOffset)) //when the object is from a property 
                                                     //in POST body. When used in service, 
                                                     //better to have 
                                                     //options.SerializerSettings.DateParseHandling = 
                                                     //Newtonsoft.Json.DateParseHandling.DateTimeOffset;
        {
            return DateOnly.FromDateTime(((DateTimeOffset)reader.Value).DateTime);
        }

        if (dateValueType == typeof(string))
        {
            return DateOnly.Parse((string)reader.Value); // DateOnly can parse 00001-01-01
        }

        if (dateValueType == typeof(DateTime)) // when the object is from a property 
                                               //in POST body from a TS client
        {
            return DateOnly.FromDateTime((DateTime)reader.Value);
        }

        throw new NotSupportedException($"Not yet support {dateValueType} in {this.GetType()}.");
    }
}