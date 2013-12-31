using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Exceptions;
using Abp.Utils.Extensions;
using Abp.Utils.Helpers;
using Castle.Core.Logging;

namespace Abp.Modules
{
    /// <summary>
    /// Used to store AbpModuleInfo objects as a dictionary.
    /// </summary>
    public class AbpModuleCollection : Dictionary<string, AbpModuleInfo>
    {
        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : AbpModule
        {
            var module = Values.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new AbpException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule)module.Instance;
        }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; } //TODO: Logging!

        /// <summary>
        /// Loads all modules.
        /// </summary>
        /// <returns>All modules</returns>
        public void LoadAll()
        {
            Logger.Debug("Loading Abp modules...");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!AbpModuleHelper.IsAbpModule(type))
                    {
                        continue;
                    }

                    var moduleInfo = CreateModuleInfo(type);
                    if (ContainsKey(moduleInfo.Name))
                    {
                        Logger.Warn("Module is loaded before: " + type.FullName);
                        continue;
                    }

                    this[moduleInfo.Name] = moduleInfo;
                    Logger.Debug("Loaded module: " + moduleInfo);
                }
            }

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", Count);
        }

        /// <summary>
        /// Sorts modules accorting to dependencies.
        /// </summary>
        /// <returns>Sorted list</returns>
        public List<AbpModuleInfo> SortByDependency()
        {
            var orderedList = new List<AbpModuleInfo>();

            foreach (var module in Values)
            {
                var index = 0; //Order of this module (will be first module if there is no dependencies of it)

                //Check all modules and place this module after all it's dependencies
                if (!module.Dependencies.IsNullOrEmpty())
                {
                    for (int i = 0; i < orderedList.Count; i++)
                    {
                        //Check for dependency
                        if (module.Dependencies.ContainsKey(orderedList[i].Name))
                        {
                            //If there is dependency, place after it
                            index = i + 1;
                        }
                    }
                }

                //Insert module the right place in the list
                orderedList.Insert(index, module);
            }

            return orderedList;
        }

        private void SetDependencies()
        {
            foreach (var module in Values)
            {
                //Check if this module has no dependecies
                if (module.ModuleAttribute.Dependencies.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var dependencyModuleName in module.ModuleAttribute.Dependencies)
                {
                    //Check if there is a module with this name
                    if (!ContainsKey(dependencyModuleName))
                    {
                        throw new AbpException(string.Format("Can not find dependent Abp module {0} for {1}.", dependencyModuleName, module.Name));
                    }

                    //Check if same dependency set before
                    if (module.Dependencies.ContainsKey(dependencyModuleName))
                    {
                        continue;
                    }

                    //Set the dependency
                    module.Dependencies[dependencyModuleName] = this[dependencyModuleName];
                }
            }
        }

        private static AbpModuleInfo CreateModuleInfo(Type type)
        {
            if (!AbpModuleHelper.IsAbpModule(type))
            {
                throw new AbpException(
                    string.Format(
                        "type {0} is not an Abp module. An Abp module must be subclass of AbpModule, must declare AbpModuleAttribute attribute and must not be abstract!",
                        type.FullName));
            }

            return new AbpModuleInfo(type, ReflectionHelper.GetSingleAttribute<AbpModuleAttribute>(type), (IAbpModule)Activator.CreateInstance(type, new object[] { }));
        }
    }
}