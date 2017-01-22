using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.PlugIns;
using Castle.Core.Logging;

namespace Abp.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager : IAbpModuleManager
    {
        public AbpModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<AbpModuleInfo> Modules => _modules.ToImmutableList();

        public ILogger Logger { get; set; }

        private AbpModuleCollection _modules;

        private readonly IIocManager _iocManager;
        private readonly IAbpPlugInManager _abpPlugInManager;

        public AbpModuleManager(IIocManager iocManager, IAbpPlugInManager abpPlugInManager)
        {
            _iocManager = iocManager;
            _abpPlugInManager = abpPlugInManager;

            Logger = NullLogger.Instance;
        }

        public virtual void Initialize(Type startupModule)
        {
            _modules = new AbpModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.Debug("Shutting down has been started");

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.Debug("Loading Abp modules...");

            List<Type> plugInModuleTypes;
            var moduleTypes = FindAllModuleTypes(out plugInModuleTypes).Distinct().ToList();

            Logger.Debug("Found " + moduleTypes.Count + " ABP modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = AbpModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);
            
            foreach (var plugInModuleType in _abpPlugInManager.PlugInSources.GetAllModules())
            {
                if (modules.AddIfNotContains(plugInModuleType))
                {
                    plugInModuleTypes.Add(plugInModuleType);
                }
            }

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = _iocManager.Resolve(moduleType) as AbpModule;
                if (moduleObject == null)
                {
                    throw new AbpInitializationException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IAbpStartupConfiguration>();

                var moduleInfo = new AbpModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                _iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in AbpModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new AbpInitializationException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
