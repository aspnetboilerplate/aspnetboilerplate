namespace Abp.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager
    {
        internal AbpModuleCollection Modules { get; private set; }

        public AbpModuleManager(AbpModuleCollection modules)
        {
            Modules = modules;
        }

        public virtual void Initialize(IAbpInitializationContext initializationContext)
        {
            Modules.LoadAll();

            var sortedModules = Modules.SortByDependency();

            sortedModules.ForEach(module => module.Instance.PreInitialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.Initialize(initializationContext));
            sortedModules.ForEach(module => module.Instance.PostInitialize(initializationContext));
        }

        public virtual void Shutdown()
        {
            var sortedModules = Modules.SortByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }
    }
}
