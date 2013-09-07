using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Exceptions;
using Abp.Modules;
using Castle.Windsor;

namespace Abp.Startup
{
    public class AbpBootstrapper : IDisposable
    {
        protected WindsorContainer IocContainer { get; private set; }

        protected IDictionary<string, AbpModuleInfo> Modules { get; set; } //TODO: Make readyonly and dictionary?

        public AbpBootstrapper()
        {
            IocContainer = new WindsorContainer();
        }

        public void Initialize()
        {
            IocContainer.Install(new AbpCoreInstaller());

            var initializationContext = new AbpInitializationContext(this);

            LoadModules();

            PreInitializeModules(initializationContext);
            InitializeModules(initializationContext);
            PostInitializeModules(initializationContext);
        }

        public void Dispose()
        {
            if (IocContainer != null)
            {
                IocContainer.Dispose();
            }
        }

        private void LoadModules()
        {
            Modules = IocContainer.Resolve<AbpModuleLoader>().LoadModules();
        }

        private void PreInitializeModules(IAbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.ModuleInstance.PreInitialize(initializationContext);
            }
        }

        private void InitializeModules(IAbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.ModuleInstance.Initialize(initializationContext);
            }
        }

        private void PostInitializeModules(IAbpInitializationContext initializationContext)
        {
            //TODO initialize in order to dependencies
            foreach (var module in Modules.Values)
            {
                module.ModuleInstance.PostInitialize(initializationContext);
            }
        }

        #region AbpInitializationContext class

        private class AbpInitializationContext : IAbpInitializationContext
        {
            public WindsorContainer IocContainer { get { return _abpBootstrapper.IocContainer; } }

            private readonly AbpBootstrapper _abpBootstrapper;

            public AbpInitializationContext(AbpBootstrapper abpBootstrapper)
            {
                _abpBootstrapper = abpBootstrapper;
            }

            public TModule GetModule<TModule>() where TModule : AbpModule
            {
                var module = _abpBootstrapper.Modules.Values.FirstOrDefault(m => m.ModuleType == typeof(TModule));
                if (module == null)
                {
                    throw new AbpException("Can not find module for " + typeof(TModule).FullName);
                }

                return (TModule)module.ModuleInstance;
            }
        }

        #endregion
    }
}
