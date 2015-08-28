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
            var type = obj.GetType();
            if (!type.IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof (Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj.GetType() == typeof(Func<TReturn>);
        }
    }
}
