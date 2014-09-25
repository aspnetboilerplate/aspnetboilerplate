using System;
using Abp.Dependency;
using Abp.Events.Bus;

namespace Abp.Startup
{
    /// <summary>
    /// This is the main class that is responsible to start entire system.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class AbpBootstrapper : IDisposable
    {
        /// <summary>
        /// Gets/sets IocManager object used by this class.
        /// </summary>
        public IocManager IocManager { get; set; }

        private AbpStartupManager _startupManager;

        private bool _isDisposed;

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
            IocManager.IocContainer.Install(new EventBusInstaller());
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (_startupManager != null)
            {
                _startupManager.Dispose();
            }
        }
    }
}
