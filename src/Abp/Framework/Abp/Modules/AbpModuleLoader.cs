using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                //Skip types those are not Abp Module
                if (!AbpModuleHelper.IsAbpModule(type))
                {
                    continue;
                }

                //Prevent multiple adding same module
                var moduleInfo = _modules.FirstOrDefault(m => m.Type == type);
                if (moduleInfo == null)
                {
                    moduleInfo = AbpModuleInfo.CreateForType(type);
                    _modules.Add(moduleInfo);
                }

                //Check for depended modules
                var dependedModuleTypes = moduleInfo.Instance.GetDependedModules();
                foreach (var dependedModuleType in dependedModuleTypes)
                {
                    FillModules(dependedModuleType.Assembly, scannedAssemblies);
                }

                Logger.Debug("Loaded module: " + moduleInfo);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                //Set dependencies according to assembly dependency
                foreach (var referencedAssemblyName in moduleInfo.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    var dependedModuleList = _modules.Where(m => m.Assembly == referencedAssembly).ToList();
                    if (dependedModuleList.Count > 0)
                    {
                        moduleInfo.Dependencies.AddRange(dependedModuleList);
                    }
                }

                //Set dependencies according to explicit dependencies
                var dependedModuleTypes = moduleInfo.Instance.GetDependedModules();
                foreach (var dependedModuleType in dependedModuleTypes)
                {
                    AbpModuleInfo dependedModule;
                    if (((dependedModule = _modules.FirstOrDefault(m => m.Type == dependedModuleType)) != null)
                        && (moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModule);
                    }
                }
            }
        }
    }
}
