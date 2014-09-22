using Abp.Startup;
using Abp.Startup.Configuration;

namespace Abp.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager
    {
        private readonly AbpModuleCollection _modules;
        private readonly AbpModuleLoader _moduleLoader;

        public AbpModuleManager(AbpModuleCollection modules, AbpModuleLoader moduleLoader)
        {
            _moduleLoader = moduleLoader;
            _modules = modules;
        }

        public virtual void Initialize(IAbpInitializationContext initializationContext)
        {
            _moduleLoader.LoadAll();
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.Configure(AbpConfiguration.Instance));
            sortedModules.ForEach(module => module.Instance.PreInitialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.Initialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.PostInitialize(initializationContext));
        }

        public virtual void Shutdown()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }
    }
}
