using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Dependency.Installers;
using Abp.Exceptions;
using Abp.Localization;
using Abp.Modules;
using Abp.Modules.Loading;
using Castle.Windsor;

namespace Abp.Startup
{
    /// <summary>
    /// This is the main class that is responsible to start entire system.
    /// It must be instantiated and initialized first.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        private IDictionary<string, AbpModuleInfo> Modules { get; set; }

        private readonly AbpInitializationContext _initializationContext;

        public AbpBootstrapper(string applicationDirectory)
        {
            _initializationContext = new AbpInitializationContext(this);
            _initializationContext.ApplicationDirectory = applicationDirectory;
        }

        public virtual void Initialize()
        {
            IocManager.Initialize();
            IocManager.IocContainer.Install(new AbpCoreInstaller());

            //TODO: Create a module manager and move all loading/sorting/initialization/shutdown methods to it!
            LoadModules();
            InitializeModules();
        }

        public void Dispose()
        {
            //TODO: Call shutdown of modules!

            IocManager.Dispose();
        }

        private void LoadModules()
        {
            Modules = IocHelper.Resolve<AbpModuleLoader>().LoadModules();
        }

        private void InitializeModules()
        {
            new AbpModuleDependencyExplorer().SetDependencies(Modules);
            var sortedModules = new AbpModuleDependencySorter().SortByDependency(Modules);

            var initializer = new AbpModuleInitializer(sortedModules, _initializationContext);
            initializer.PreInitializeModules();
            initializer.InitializeModules();
            initializer.PostInitializeModules();
        }

        #region AbpInitializationContext class

        private class AbpInitializationContext : IAbpInitializationContext
        {
            public WindsorContainer IocContainer { get { return IocManager.IocContainer; } }
            
            public string ApplicationDirectory { get; set; }

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
