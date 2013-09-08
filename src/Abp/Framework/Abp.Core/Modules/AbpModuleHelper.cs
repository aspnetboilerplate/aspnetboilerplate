using System;

namespace Abp.Modules
{
    internal static class AbpModuleHelper
    {
        public static bool IsAbpModule(Type type)
        {
            return (!type.IsAbstract && typeof(IAbpModule).IsAssignableFrom(type) && type.IsDefined(typeof(AbpModuleAttribute), true));
        }
    }
}