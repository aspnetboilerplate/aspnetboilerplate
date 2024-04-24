using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abp.Extensions;
using Abp.Reflection;

namespace Abp.Json.SystemTextJson
{
    public class AbpJsonConverterForType: JsonConverter<Type>
    {
        public override Type Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var assemblyQualifiedName = reader.GetString();
            if (assemblyQualifiedName.IsNullOrEmpty())
            {
                throw new Exception("AssemblyQualifiedName is empty!");
            }
            
            return Type.GetType(assemblyQualifiedName);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Type value,
            JsonSerializerOptions options
        )
        {
            var assemblyQualifiedName = TypeHelper.SerializeType(value).ToString();
            writer.WriteStringValue(assemblyQualifiedName);
        }
    }
}