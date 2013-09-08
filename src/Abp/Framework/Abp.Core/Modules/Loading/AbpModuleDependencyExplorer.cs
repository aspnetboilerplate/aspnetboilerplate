using System.Collections.Generic;
using Abp.Exceptions;
using Abp.Utils.Extensions;

namespace Abp.Modules.Loading
{
    /// <summary>
    /// This class is used to set dependencies of modules to <see cref="AbpModuleInfo.Dependencies"/> property.
    /// </summary>
    internal class AbpModuleDependencyExplorer
    {
        /// <summary>
        /// Sets <see cref="AbpModuleInfo.Dependencies"/> properties of given modules.
        /// </summary>
        /// <param name="modules">All modules</param>
        public void SetDependencies(IDictionary<string, AbpModuleInfo> modules)
        {
            foreach (var module in modules.Values)
            {
                //Check if this module has no dependecies
                if (module.ModuleAttribute.Dependencies.IsNullOrEmpty())
                {
                    continue;
                }
                
                foreach (var dependencyModuleName in module.ModuleAttribute.Dependencies)
                {
                    //Check if there is a module with this name
                    if (!modules.ContainsKey(dependencyModuleName))
                    {
                        throw new AbpException(string.Format("Can not find dependent Abp module {0} for {1}.", dependencyModuleName, module.Name));
                    }

                    //Check if same dependency set before
                    if (module.Dependencies.ContainsKey(dependencyModuleName))
                    {
                        continue;
                    }

                    //Set the dependency
                    module.Dependencies[dependencyModuleName] = modules[dependencyModuleName];
                }
            }
        }
    }
}