using Abp.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Abp.Domain.Entities
{
    public static class ExtendableObjectExtensions
    {
        public static T GetData<T>([NotNull] this IExtendableObject extendableObject, string name)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));

            if (extendableObject.ExtensionData == null)
            {
                return default(T);
            }

            var json = JObject.Parse(extendableObject.ExtensionData);
            var prop = json[name];

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T)))
            {
                return prop.Value<T>();
            }
            else
            {
                return (T)prop.ToObject(typeof(T));
            }
        }

        public static void SetData([NotNull] this IExtendableObject extendableObject, string name, object value)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));

            if (extendableObject.ExtensionData == null)
            {
                extendableObject.ExtensionData = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            if (value == null || TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
            {
                json[name] = new JValue(value);
            }
            else
            {
                json[name] = JToken.FromObject(value);
            }

            extendableObject.ExtensionData = json.ToString(Formatting.None);
        }

        //TODO: string[] GetExtendedPropertyNames(...)
    }
}