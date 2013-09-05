using System.Collections.Generic;
using Castle.Windsor;

namespace Abp.Web.Modules
{
    public class AbpModuleManager
    {
        public static AbpModuleManager Instance { get { return _instance; } }

        public IList<AbpModule> Modules { get; set; } //TODO: Make readyonly and dictionary?

        private static readonly AbpModuleManager _instance = new AbpModuleManager();

        internal WindsorContainer IocContainer { get; set; }

        private AbpModuleManager()
        {
            Modules = new List<AbpModule>();
        }

        public void RegisterModule(AbpModule module)
        {
            Modules.Add(module);
        }

        internal void PreInitializeModules(AbpInitializationContext initializationContext)
        {
            foreach (var module in Modules)
            {
                module.PreInitialize(initializationContext);
            }
        }

        internal void InitializeModules(AbpInitializationContext initializationContext)
        {
            foreach (var module in Modules)
            {
                module.Initialize(initializationContext);
            }
        }

        internal void PostInitializeModules(AbpInitializationContext initializationContext)
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

        internal AbpInitializationContext(WindsorContainer iocContainer)
        {
            IocContainer = iocContainer;
        }
    }
}