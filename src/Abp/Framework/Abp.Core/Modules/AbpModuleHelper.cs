using System;

namespace Abp.Modules
{
    /// <summary>
    /// Helper methods for Abp modules
    /// </summary>
    internal static class AbpModuleHelper
    {
        /// <summary>
        /// Checks if given type is an Abp module class.
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsAbpModule(Type type)
        {
            return
                type.IsPublic &&
                type.IsClass &&
                !type.IsAbstract &&
                typeof (IAbpModule).IsAssignableFrom(type) &&
                type.IsDefined(typeof (AbpModuleAttribute), false);
        }
    }
}