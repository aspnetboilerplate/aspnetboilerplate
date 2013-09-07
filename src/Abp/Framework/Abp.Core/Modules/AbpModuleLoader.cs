using System;
using System.Collections.Generic;
using Abp.Exceptions;
using Abp.Utils.Helpers;
using Castle.Core.Logging;

namespace Abp.Modules
{
    public class AbpModuleLoader
    {
        public ILogger Logger { get; set; }

        public Dictionary<string, AbpModuleInfo> LoadModules()
        {
            Logger.Debug("Loading Abp modules...");

            var modules = new Dictionary<string, AbpModuleInfo>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (AbpModuleHelper.IsAbpModule(type))
                    {
                        var moduleInfo = CreateModuleInfo(type);
                        if (modules.ContainsKey(moduleInfo.Name))
                        {
                            Logger.Warn("Module is loaded before! " + type.FullName);
                            continue;
                        }

                        modules[moduleInfo.Name] = moduleInfo;
                        Logger.Debug("Loaded module: " + moduleInfo);
                    }
                }
            }

            Logger.DebugFormat("{0} modules loaded.", modules.Count);
            return modules;
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

            return new AbpModuleInfo
                       {
                           ModuleType = type,
                           ModuleAttribute = ReflectionHelper.GetSingleAttribute<AbpModuleAttribute>(type),
                           ModuleInstance = (AbpModule) Activator.CreateInstance(type, new object[] {})
                       };
        }
    }
}