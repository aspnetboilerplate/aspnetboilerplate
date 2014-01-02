using System;
using Abp.Dependency;
using Abp.Localization;
using Abp.Modules;

namespace Abp.Startup
{
    /// <summary>
    /// This is the main class that is responsible to start entire system.
    /// It must be instantiated and initialized first.
    /// It starts Dependency Injection system.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        private AbpApplicationManager _applicationManager;
        
        /// <summary>
        /// Initializes the system.
        /// </summary>
        public virtual void Initialize()
        {
            IocManager.Instance.IocContainer.Install(new AbpStartupInstaller());
            IocManager.Instance.IocContainer.Install(new AbpModuleSystemInstaller());
            IocManager.Instance.IocContainer.Install(new LocalizationInstaller());

            _applicationManager = IocHelper.Resolve<AbpApplicationManager>();
            _applicationManager.Initialize();
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public virtual void Dispose()
        {
            _applicationManager.Dispose();
            IocManager.Instance.Dispose();
        }
    }
}
