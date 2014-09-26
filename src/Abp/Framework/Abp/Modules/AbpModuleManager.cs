using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Startup;
using Castle.Core.Logging;

namespace Abp.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager
    {
        public ILogger Logger { get; set; }
        
        public IModuleFinder ModuleFinder { get; set; }

        private readonly AbpModuleCollection _modules;

        private readonly IIocManager _iocManager;

        public AbpModuleManager(AbpModuleCollection modules, IIocManager iocManager)
        {
            _modules = modules;
            _iocManager = iocManager;
            Logger = NullLogger.Instance;
            ModuleFinder = new DefaultModuleFinder();
        }

        public virtual void InitializeModules()
        {
            var initializationContext = new AbpInitializationContext(_iocManager, _modules);

            LoadAll();

            var sortedModules = _modules.GetSortedModuleListByDependency();
            
            sortedModules.ForEach(module => module.Instance.PreInitialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.Initialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.PostInitialize(initializationContext));
        }

        public virtual void ShutdownModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();

            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }

        private void LoadAll()
        {
            Logger.Debug("Loading Abp modules...");

            _modules.AddRange(ModuleFinder.FindAll());

            var startupModuleIndex = _modules.FindIndex(m => m.Type == typeof(AbpStartupModule));
            if (startupModuleIndex > 0)
            {
                var startupModule = _modules[startupModuleIndex];
                _modules.RemoveAt(startupModuleIndex);
                _modules.Insert(0, startupModule);
            }

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
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
