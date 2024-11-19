using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Json;

public class AbpMvcContractResolver : DefaultContractResolver
{
    private List<string> InputDateTimeFormats { get; set; }
    private string OutputDateTimeFormat { get; set; }

    public AbpMvcContractResolver(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
    {
        InputDateTimeFormats = inputDateTimeFormats ?? new List<string>();
        OutputDateTimeFormat = outputDateTimeFormat;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        ModifyProperty(member, property);

        return property;
    }

    protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
    {
        if (!AbpDateTimeConverter.ShouldNormalize(member, property))
        {
            return;
        }

        property.Converter = new AbpDateTimeConverter(InputDateTimeFormats, OutputDateTimeFormat);
    }
}