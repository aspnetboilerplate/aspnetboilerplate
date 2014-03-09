using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var scannedAssemlies = new List<Assembly>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                FillModules(assembly, scannedAssemlies);
            }

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private void FillModules(Assembly assembly, List<Assembly> scannedAssemblies)
        {
            if (scannedAssemblies.Contains(assembly))
            {
                return;
            }

            scannedAssemblies.Add(assembly);
            var referencedAssemblyNames = assembly.GetReferencedAssemblies();
            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                var referencedAssembly = Assembly.Load(referencedAssemblyName);
                FillModules(referencedAssembly, scannedAssemblies);
            }

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

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules.Values)
            {
                foreach (var referencedAssemblyName in moduleInfo.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    var dependedModuleList = _modules.Values.Where(m => m.Assembly == referencedAssembly).ToList();
                    if (dependedModuleList.Count > 0)
                    {
                        moduleInfo.Dependencies.AddRange(dependedModuleList);
                    }
                }
            }
        }
    }
}
