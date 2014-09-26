using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Startup;
using Abp.Startup.Configuration;
using Castle.Core.Logging;

namespace Abp.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager : IAbpModuleManager
    {
        public ILogger Logger { get; set; }
        
        public IModuleFinder ModuleFinder { get; set; }

        private readonly AbpModuleCollection _modules;

        private readonly IIocManager _iocManager;

        public AbpModuleManager(IIocManager iocManager)
        {
            _modules = new AbpModuleCollection();
            _iocManager = iocManager;
            Logger = NullLogger.Instance;
            ModuleFinder = new DefaultModuleFinder();
        }

        public virtual void InitializeModules()
        {
            LoadAll();

            var sortedModules = _modules.GetSortedModuleListByDependency();

            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
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
            
            var moduleTypes = ModuleFinder.FindAll();

            //Register to IOC container.
            foreach (var moduleType in moduleTypes)
            {
                if (!_iocManager.IsRegistered(moduleType))
                {
                    _iocManager.Register(moduleType, DependencyLifeStyle.Singleton);
                }
            }

            //Add to module collection
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = (AbpModule) _iocManager.Resolve(moduleType);
                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = AbpConfiguration.Instance;

                var moduleInfo = new AbpModuleInfo(moduleObject);

                _modules.Add(moduleInfo);
                
                Logger.DebugFormat("Loaded module: " + moduleInfo);
            }

            //AbpStartupModule must be the first module
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
