using System;
using Abp.Exceptions;
using Abp.Utils.Extensions.Collections;
using Castle.Core.Logging;

namespace Abp.Modules
{
    /// <summary>
    /// This class is used to Load modules on startup.
    /// </summary>
    public class AbpModuleLoader
    {
        public ILogger Logger { get; set; }

        private readonly AbpModuleCollection _modules;
        
        public AbpModuleLoader(AbpModuleCollection modules)
        {
            _modules = modules;
            Logger = NullLogger.Instance;
        }

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

                    var moduleInfo = AbpModuleInfo.CreateForType(type);
                    if (_modules.ContainsKey(moduleInfo.Name))
                    {
                        Logger.Warn("Module is loaded before: " + type.FullName);
                        continue;
                    }

                    _modules[moduleInfo.Name] = moduleInfo;
                    Logger.Debug("Loaded module: " + moduleInfo);
                }
            }

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules.Values)
            {
                //Check if this module has no dependecies
                if (moduleInfo.ModuleAttribute.Dependencies.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var dependencyModuleName in moduleInfo.ModuleAttribute.Dependencies)
                {
                    //Check if there is a module with this name
                    if (!_modules.ContainsKey(dependencyModuleName))
                    {
                        throw new AbpException(string.Format("Can not find dependent Abp module {0} for {1}.", dependencyModuleName, moduleInfo.Name));
                    }

                    //Check if same dependency set before
                    if (moduleInfo.Dependencies.ContainsKey(dependencyModuleName))
                    {
                        continue;
                    }

                    //Set the dependency
                    moduleInfo.Dependencies[dependencyModuleName] = _modules[dependencyModuleName];
                }
            }
        }
    }
}
