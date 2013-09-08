using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency.Installers;
using Abp.Exceptions;
using Abp.Modules;
using Abp.Modules.Loading;
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
            LoadModules();
            new AbpModuleDependencyExplorer().SetDependencies(Modules);
            var sortedModules = new AbpModuleDependencySorter().SortByDependency(Modules);
            var initializer = new AbpModuleInitializer(sortedModules, new AbpInitializationContext(this));
            initializer.PreInitializeModules();
            initializer.InitializeModules();
            initializer.PostInitializeModules();
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
                var module = _abpBootstrapper.Modules.Values.FirstOrDefault(m => m.Type == typeof(TModule));
                if (module == null)
                {
                    throw new AbpException("Can not find module for " + typeof(TModule).FullName);
                }

                return (TModule)module.Instance;
            }
        }

        #endregion
    }
}
