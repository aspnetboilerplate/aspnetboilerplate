using System;

namespace Abp.Reflection
{
    /// <summary>
    /// Some simple type-checking methods used internally.
    /// </summary>
    internal static class TypeHelper
    {
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof (Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }

        public static bool IsPrimitiveIncludingNullable(Type type)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GenericTypeArguments[0].IsPrimitive;
            }

            return false;
        }
    }
}
