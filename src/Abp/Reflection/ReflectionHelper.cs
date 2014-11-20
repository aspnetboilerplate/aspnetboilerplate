using System;

namespace Abp.Reflection
{
    /// <summary>
    /// Defines helper methods for reflection.
    /// </summary>
    internal static class ReflectionHelper
    {
        /// <summary>
        /// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenType.BaseType, genericType);
        }
    }
}