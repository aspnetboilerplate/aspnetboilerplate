using System;
using System.Reflection;
using Abp.Utils.Helpers;

namespace Abp.Modules
{
    internal static class AbpModuleHelper
    {
        public static bool IsAbpModule(Type type)
        {
            return (!type.IsAbstract && type.IsSubclassOf(typeof(AbpModule)) && type.IsDefined(typeof(AbpModuleAttribute), true));
        }

        public static string GetModuleName(AbpModule module)
        {
            return GetModuleName(module.GetType());
        }

        public static string GetModuleName<TModule>() where TModule: AbpModule
        {
            return GetModuleName(typeof (TModule));
        } 

        private static string GetModuleName(Type type)
        {
            //TODO: Check attr and throw exception
            return ReflectionHelper.GetSingleAttribute<AbpModuleAttribute>(type).Name;
        }
    }
}