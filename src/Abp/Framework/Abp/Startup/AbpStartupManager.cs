using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup
{
    /// <summary>
    /// This class is the main class that starts the application.
    /// This is used by <see cref="AbpBootstrapper"/> to start the application.
    /// </summary>
    public class AbpStartupManager
    {
        private readonly IocManager _iocManager;
        private readonly AbpModuleManager _moduleManager;
        private readonly AbpModuleCollection _modules;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpStartupManager(IocManager iocManager, AbpModuleManager moduleManager, AbpModuleCollection modules)
        {
            _iocManager = iocManager;
            _moduleManager = moduleManager;
            _modules = modules;
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public virtual void Initialize()
        {
            var initializationContext = new AbpInitializationContext(_iocManager, _modules);
            _moduleManager.Initialize(initializationContext);
        }

        /// <summary>
        /// Disposes/shutdowns the application.
        /// </summary>
        public virtual void Dispose()
        {
            _moduleManager.Shutdown();
        }
    }
}
