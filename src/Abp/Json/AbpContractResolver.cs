using System;
using System.Reflection;
using Abp.Reflection;
using Abp.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Json
{
    public class AbpContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            if (AbpDateTimeConverter.ShouldNormalize(member, property))
            {
                property.Converter = new AbpDateTimeConverter();
            }
        }
    }
}
