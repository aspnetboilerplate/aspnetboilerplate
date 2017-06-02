using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Modules;
using Abp.Reflection;

namespace Abp.Web
{
    /// <summary>
    /// This class is used to get assemblies, which contain at least one AbpModule-derived class.
    /// </summary>
    public class AbpModuleAssemblyFindler : IAssemblyFinder
    {
        private List<Assembly> _assemblies;

        private readonly object _syncLock = new object();

        /// <summary>
        /// This return assemblies, which contain at least one AbpModule-derived class
        /// </summary>
        /// <returns>List of assemblies</returns>
        public List<Assembly> GetAllAssemblies()
        {
            if (_assemblies == null)
            {
                lock (_syncLock)
                {
                    if (_assemblies == null)
                    {
                        _assemblies = GetAbpModuleAssembliesRecursive();
                    }
                }
            }

            return _assemblies;
        }

        /// <summary>
        /// Retrieves all of the AbpModules and their dependencies
        /// </summary>
        /// <returns></returns>
        private List<Assembly> GetAbpModuleAssembliesRecursive()
        {
            var currentAssembly = Assembly.GetCallingAssembly();

            var topLevelAbpModules = currentAssembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(AbpModule)))
                .ToList();

            var allModules = new List<Type>();

            foreach (var module in topLevelAbpModules)
            {
                RecursiveAbpModulesLookup(module, allModules);
            }

            var assemblies = allModules.Select(module => module.Assembly).Distinct().ToList();
            return assemblies;
        }

        /// <summary>
        /// Traverse method for getting a modules.
        /// Recursion termination condition: <paramref name="allModules"/> already containts <paramref name="moduleType"/>.
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="allModules"></param>
        private void RecursiveAbpModulesLookup(Type moduleType, ICollection<Type> allModules)
        {
            // Add current module
            if (allModules.AddIfNotContains(moduleType))
            {
                // Get dependencies
                var modules = GetDependentModulesFromAttribute(moduleType);

                // Step in
                if (modules.Any())
                    modules.ForEach(m => RecursiveAbpModulesLookup(m, allModules));
            }
        }

        private static List<Type> GetDependentModulesFromAttribute(Type moduleType)
        {
            var dependencies = moduleType.GetCustomAttribute<DependsOnAttribute>(false);
            return dependencies?.DependedModuleTypes.ToList() ?? new List<Type>();
        }
    }
}
