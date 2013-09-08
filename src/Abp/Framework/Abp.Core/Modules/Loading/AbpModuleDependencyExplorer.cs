using System.Collections.Generic;
using Abp.Exceptions;

namespace Abp.Modules.Loading
{
    /// <summary>
    /// This class is used to set dependencies of modules. See AbpModuleInfo.Dependencies
    /// </summary>
    internal class AbpModuleDependencyExplorer
    {
        public void SetDependencies(IDictionary<string, AbpModuleInfo> modules)
        {
            foreach (var module in modules.Values)
            {
                if (module.ModuleAttribute.Dependencies == null)
                {
                    continue;
                }

                foreach (var dependencyModuleName in module.ModuleAttribute.Dependencies)
                {
                    if (!modules.ContainsKey(dependencyModuleName))
                    {
                        throw new AbpException(string.Format("Can not find dependent Abp module {0} for {1}.", dependencyModuleName, module.Name));
                    }

                    if (module.Dependencies.ContainsKey(dependencyModuleName))
                    {
                        continue;
                    }

                    module.Dependencies[dependencyModuleName] = modules[dependencyModuleName];
                }
            }
        }
    }
}