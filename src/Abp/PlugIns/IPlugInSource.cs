using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Modules;

namespace Abp.PlugIns
{
    public interface IPlugInSource
    {
        List<Type> GetModules();
    }

    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(AbpModule.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}