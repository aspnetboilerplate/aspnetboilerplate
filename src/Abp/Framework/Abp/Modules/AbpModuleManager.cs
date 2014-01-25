using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Startup;

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

            IocManager.Instance.AddConventionalRegisterer(new BasicConventionalRegisterer()); //TODO: Remove somewhere else!
            sortedModules.ForEach(module => module.Instance.PreInitialize(initializationContext));

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig() { InstallInstallers = false });  //TODO: Remove somewhere else!
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
