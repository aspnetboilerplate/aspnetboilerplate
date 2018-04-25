using System.Collections.Generic;
using Abp.Reflection;
using ChangeTracking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Abp.Domain.Entities
{
    public static class ExtendableObjectExtensions
    {
        public static T GetData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name, bool handleType = false)
        {
            return extendableObject.GetData<T>(
                name,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        public static T GetData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name, [CanBeNull] JsonSerializer jsonSerializer)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));
            Check.NotNull(name, nameof(name));

            if (extendableObject.ExtensionData == null)
            {
                return default(T);
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            var prop = json[name];
            if (prop == null)
            {
                return default(T);
            }

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T)))
            {
                return prop.Value<T>();
            }
            else
            {
                return (T)prop.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
            }
        }

        public static void SetData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name, [CanBeNull] T value, bool handleType = false)
        {
            extendableObject.SetData(
                name,
                value,
                handleType
                    ? new JsonSerializer {TypeNameHandling = TypeNameHandling.All}
                    : JsonSerializer.CreateDefault()
            );
        }

        public static void SetData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name, [CanBeNull] T value, [CanBeNull] JsonSerializer jsonSerializer)
        {
            Check.NotNull(extendableObject, nameof(extendableObject));
            Check.NotNull(name, nameof(name));

            if (jsonSerializer == null)
            {
                jsonSerializer = JsonSerializer.CreateDefault();
            }

            if (extendableObject.ExtensionData == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    return;
                }

                extendableObject.ExtensionData = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
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
                json[name] = JToken.FromObject(value, jsonSerializer);
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

        /// <summary>
        /// Return a trackable POJO object, which will call SetData automatically when its properties changed.
        /// </summary>
        public static T GetTrackableData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name,
            [CanBeNull] JsonSerializer jsonSerializer) where T : class
        {
            var data = extendableObject.GetData<T>(name, jsonSerializer);
            if (data == null)
                return null;

            var obj = data.AsTrackable();

            obj.CastToIChangeTrackable()
                .PropertyChanged += (sender, args) =>
            {
                var trackable = (IChangeTrackable<T>)sender;
                trackable.AcceptChanges();

                extendableObject.SetData(name, trackable.GetOriginal());
            };
            return obj;
        }

        /// <summary>
        /// Return a trackable POJO class, which will call SetData automatically when its properties changed.
        /// </summary>
        public static T GetTrackableData<T>([NotNull] this IExtendableObject extendableObject, [NotNull] string name,
            bool handleType = false) where T : class
        {
            return extendableObject.GetTrackableData<T>(
                name,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }

        //TODO: string[] GetExtendedPropertyNames(...)
    }
}