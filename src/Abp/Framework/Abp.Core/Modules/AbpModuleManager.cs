using Abp.Startup;

namespace Abp.Modules
{
    public class AbpModuleManager
    {
        internal AbpModuleCollection Modules { get; private set; }

        public AbpModuleManager(AbpModuleCollection modules)
        {
            Modules = modules;
        }

        public virtual void InitializeModules(IAbpInitializationContext initializationContext)
        {
            Modules.LoadAll();

            var sortedModules = Modules.SortByDependency();

            sortedModules.ForEach(module => module.Instance.PreInitialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.Initialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.PostInitialize(initializationContext));
        }

        public virtual void ShutdownModules()
        {
            var sortedModules = Modules.SortByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }
    }
}
