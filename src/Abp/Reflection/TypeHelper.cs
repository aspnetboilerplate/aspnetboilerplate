using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Abp.Reflection
{
    /// <summary>
    /// Some simple type-checking methods used internally.
    /// </summary>
    internal static class TypeHelper
    {
        private static readonly IReadOnlyList<string> SystemAssemblyNames = new List<string> { "mscorlib", "System.Private.CoreLib" };
        
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }

        public static bool IsPrimitiveExtendedIncludingNullable(Type type, bool includeEnums = false)
        {
            if (IsPrimitiveExtended(type, includeEnums))
            {
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveExtended(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        private static bool IsPrimitiveExtended(Type type, bool includeEnums)
        {
            if (type.GetTypeInfo().IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.GetTypeInfo().IsEnum)
            {
                return true;
            }

            return type == typeof (string) ||
                   type == typeof (decimal) ||
                   type == typeof (DateTime) ||
                   type == typeof (DateTimeOffset) ||
                   type == typeof (TimeSpan) ||
                   type == typeof (Guid);
        }
        
        public static StringBuilder SerializeType(Type type, bool withAssemblyName = true, StringBuilder typeNameBuilder = null)
        {
            typeNameBuilder = typeNameBuilder ?? new StringBuilder();

            if (type.DeclaringType != null)
            {
                SerializeType(type.DeclaringType, false, typeNameBuilder).Append('+');
            }
            else if (type.Namespace != null)
            {
                typeNameBuilder.Append(type.Namespace).Append('.');
            }

            typeNameBuilder.Append(type.Name);

            if (type.GenericTypeArguments.Length > 0)
            {
                SerializeTypes(type.GenericTypeArguments, '[', ']', typeNameBuilder);
            }

            if (!withAssemblyName)
            {
                return typeNameBuilder;
            }

            var assemblyName = type.GetTypeInfo().Assembly.GetName().FullName;

            if (!SystemAssemblyNames.Contains(assemblyName))
            {
                typeNameBuilder.Append(", ").Append(assemblyName);
            }

            return typeNameBuilder;
        }

        private static StringBuilder SerializeTypes(Type[] types, char beginTypeDelimiter = '"', char endTypeDelimiter = '"', StringBuilder typeNamesBuilder = null)
        {
            if (types == null)
            {
                return null;
            }

            if (typeNamesBuilder == null)
            {
                typeNamesBuilder = new StringBuilder();
            }

            typeNamesBuilder.Append('[');

            for (int i = 0; i < types.Length; i++)
            {
                typeNamesBuilder.Append(beginTypeDelimiter);
                SerializeType(types[i], true, typeNamesBuilder);
                typeNamesBuilder.Append(endTypeDelimiter);

                if (i != types.Length - 1)
                {
                    typeNamesBuilder.Append(',');
                }
            }

            return typeNamesBuilder.Append(']');
        }
    }
}
