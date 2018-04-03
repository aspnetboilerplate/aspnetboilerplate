using System.Reflection;
using Abp.Reflection;
using Abp.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Json
{
    public class AbpContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(member) == null)
            {
                property.Converter = new AbpDateTimeConverter();
            }
        }
    }
}