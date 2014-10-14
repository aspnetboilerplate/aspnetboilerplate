using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Dependency.Installers;
using Abp.Modules;

namespace Abp
{
    /// <summary>
    /// This is the main class that is responsible to start entire ABP system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        /// <summary>
        /// Gets/sets _iocManager object used by this class.
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private IAbpModuleManager _moduleManager;

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        public AbpBootstrapper()
            : this(IocManager.Instance)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="iocManager">IOC manager that is used to bootstrap the ABP system</param>
        public AbpBootstrapper(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        /// <summary>
        /// Initializes the complete system.
        /// </summary>
        public virtual void Initialize()
        {
            _iocManager.IocContainer.Install(new AbpCoreInstaller());

            _iocManager.Resolve<AbpStartupConfiguration>().Initialize();

            _moduleManager = _iocManager.Resolve<IAbpModuleManager>();
            _moduleManager.InitializeModules();
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (_moduleManager != null)
            {
                _moduleManager.ShutdownModules();
            }
        }
    }
}
