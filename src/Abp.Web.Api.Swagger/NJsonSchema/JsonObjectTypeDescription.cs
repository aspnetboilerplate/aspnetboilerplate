//-----------------------------------------------------------------------
// <copyright file="JsonObjectTypeDescription.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Reflection;

namespace NJsonSchema
{
    /// <summary>Gets JSON information about a .NET type. </summary>
    public class JsonObjectTypeDescription
    {
        /// <summary>Creates a <see cref="JsonObjectTypeDescription"/> from a <see cref="Type"/>. </summary>
        /// <param name="type">The type. </param>
        /// <returns>The <see cref="JsonObjectTypeDescription"/>. </returns>
        public static JsonObjectTypeDescription FromType(Type type)
        {
            if (type.GetTypeInfo().IsEnum)
                return new JsonObjectTypeDescription(JsonObjectType.String, true);

            if ((type == typeof(int) || type == typeof(long) || type == typeof(short)) ||
                (type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort)))
                return new JsonObjectTypeDescription(JsonObjectType.Integer, true);

            if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
                return new JsonObjectTypeDescription(JsonObjectType.Number, true);

            if (type == typeof(bool))
                return new JsonObjectTypeDescription(JsonObjectType.Boolean, true);

            if (type == typeof(string))
                return new JsonObjectTypeDescription(JsonObjectType.String, false);
            if (type == typeof(Guid))
                return new JsonObjectTypeDescription(JsonObjectType.String, false);

            if (type == typeof(DateTime))
                return new JsonObjectTypeDescription(JsonObjectType.String, true, false, JsonFormatStrings.DateTime);

            if (type == typeof(Uri))
                return new JsonObjectTypeDescription(JsonObjectType.String, true, false, JsonFormatStrings.Uri);

            if (IsDictionaryType(type))
                return new JsonObjectTypeDescription(JsonObjectType.Object, false, true);

            if (IsArrayType(type))
                return new JsonObjectTypeDescription(JsonObjectType.Array, false);

            if (type.Name == "Nullable`1")
            {
                var typeDescription = FromType(type.GenericTypeArguments[0]);
                typeDescription.IsAlwaysRequired = false; 
                return typeDescription;
            }

            return new JsonObjectTypeDescription(JsonObjectType.Object, false);
        }

        private JsonObjectTypeDescription(JsonObjectType type, bool isAlwaysRequired, bool isDictionary = false, string format = null)
        {
            Type = type;
            IsAlwaysRequired = isAlwaysRequired;
            Format = format;
            IsDictionary = isDictionary; 
        }

        /// <summary>Gets the type. </summary>
        public JsonObjectType Type { get; private set; }

        /// <summary>Gets a value indicating whether the type must always required. </summary>
        public bool IsAlwaysRequired { get; private set; }

        /// <summary>Gets a value indicating whether the object is a generic dictionary.</summary>
        public bool IsDictionary { get; private set; }

        /// <summary>Gets the format string. </summary>
        public string Format { get; private set; }

        private static bool IsArrayType(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        private static bool IsDictionaryType(Type type)
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }
}