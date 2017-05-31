using System.Collections.Generic;
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

        public static void SetData<T>([NotNull] this IExtendableObject extendableObject, string name, T value)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));

            if (extendableObject.ExtensionData == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    return;
                }

                extendableObject.ExtensionData = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (json[name] != null)
                {
                    json.Remove(name);
                }
            }
            else if (TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
            {
                json[name] = new JValue(value);
            }
            else
            {
                json[name] = JToken.FromObject(value);
            }

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionData = data;
        }

        public static bool RemoveData([NotNull] this IExtendableObject extendableObject, string name)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));

            if (extendableObject.ExtensionData == null)
            {
                return false;
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            var token = json[name];
            if (token == null)
            {
                return false;
            }

            json.Remove(name);

            var data = json.ToString(Formatting.None);
            if (data == "{}")
            {
                data = null;
            }

            extendableObject.ExtensionData = data;

            return true;
        }

        //TODO: string[] GetExtendedPropertyNames(...)
    }
}