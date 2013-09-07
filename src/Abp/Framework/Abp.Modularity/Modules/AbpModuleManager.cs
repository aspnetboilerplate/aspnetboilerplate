using System.Collections.Generic;
using Castle.Windsor;

namespace Abp.Web.Modules
{
    public class AbpModuleManager
    {
        public static AbpModuleManager Instance { get { return _instance; } }

        public IList<AbpModule> Modules { get; set; } //TODO: Make readyonly and dictionary?

        private static readonly AbpModuleManager _instance = new AbpModuleManager();

        public WindsorContainer IocContainer { get; set; }

        private AbpModuleManager()
        {
            Modules = new List<AbpModule>();
        }

        public void RegisterModule(AbpModule module)
        {
            Modules.Add(module);
        }

        public void PreInitializeModules(AbpInitializationContext initializationContext)
        {
            foreach (var module in Modules)
            {
                module.PreInitialize(initializationContext);
            }
        }

        public void InitializeModules(AbpInitializationContext initializationContext)
        {
            foreach (var module in Modules)
            {
                module.Initialize(initializationContext);
            }
        }

        public void PostInitializeModules(AbpInitializationContext initializationContext)
        {
            foreach (var module in Modules)
            {
                module.PostInitialize(initializationContext);
            }
        }
    }

    public class AbpInitializationContext
    {
        public WindsorContainer IocContainer { get; private set; }

        public AbpInitializationContext(WindsorContainer iocContainer)
        {
            IocContainer = iocContainer;
        }
    }
}