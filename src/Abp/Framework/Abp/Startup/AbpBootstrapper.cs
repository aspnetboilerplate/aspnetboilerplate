using System;
using Abp.Dependency;

namespace Abp.Startup
{
    /// <summary>
    /// This is the main class that is responsible to start entire ABP system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        /// <summary>
        /// Gets/sets IocManager object used by this class.
        /// </summary>
        public IocManager IocManager { get; set; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private AbpStartupManager _startupManager;

        /// <summary>
        /// Creates a new <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        public AbpBootstrapper()
        {
            IocManager = IocManager.Instance;
        }
        
        /// <summary>
        /// Initializes the complete system.
        /// </summary>
        public virtual void Initialize()
        {
            RegisterCoreDependencies();

            _startupManager = IocManager.IocContainer.Resolve<AbpStartupManager>();
            _startupManager.Initialize();
        }

        /// <summary>
        /// Registers core dependencies for starting of ABP system.
        /// </summary>
        protected virtual void RegisterCoreDependencies()
        {
            IocManager.IocContainer.Install(new AbpStartupInstaller());
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

            if (_startupManager != null)
            {
                _startupManager.Dispose();
            }
        }
    }
}
