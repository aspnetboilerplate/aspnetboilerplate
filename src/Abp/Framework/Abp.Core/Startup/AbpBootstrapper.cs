using System;
using Abp.Dependency;
using Abp.Localization;
using Abp.Modules;
using Castle.Windsor.Installer;

namespace Abp.Startup
{
    /// <summary>
    /// This is the main class that is responsible to start entire system.
    /// It must be instantiated and initialized first.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        private readonly string _applicationDirectory;

        private AbpApplicationManager _applicationManager;

        public AbpBootstrapper(string applicationDirectory) //TODO: Can we remove application directory?
        {
            _applicationDirectory = applicationDirectory;
        }

        public virtual void Initialize()
        {
            IocManager.Instance.IocContainer.Install(new AbpStartupInstaller());
            IocManager.Instance.IocContainer.Install(new AbpModuleSystemInstaller());
            IocManager.Instance.IocContainer.Install(new LocalizationInstaller());

            _applicationManager = IocHelper.Resolve<AbpApplicationManager>(new { applicationDirectory = _applicationDirectory });
            _applicationManager.Initialize();
        }

        public virtual void Dispose()
        {
            _applicationManager.Dispose();

            IocManager.Instance.Dispose();
        }
    }
}
