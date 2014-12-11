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
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private IAbpModuleManager _moduleManager;

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        public AbpBootstrapper()
            : this(Dependency.IocManager.Instance)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        /// <param name="iocManager">IIocManager that is used to bootstrap the ABP system</param>
        public AbpBootstrapper(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        /// <summary>
        /// Initializes the ABP system.
        /// </summary>
        public virtual void Initialize()
        {
            IocManager.IocContainer.Install(new AbpCoreInstaller());

            IocManager.Resolve<AbpStartupConfiguration>().Initialize();

            _moduleManager = IocManager.Resolve<IAbpModuleManager>();
            _moduleManager.InitializeModules();
        }

        /// <summary>
        /// Disposes the ABP system.
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
