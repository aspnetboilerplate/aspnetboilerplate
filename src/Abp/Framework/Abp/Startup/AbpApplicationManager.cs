using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup
{
    /// <summary>
    /// This class is the main class that manages an application and modules.
    /// </summary>
    public class AbpApplicationManager
    {
        private readonly AbpModuleManager _moduleManager;
        private readonly AbpModuleCollection _modules;

        public AbpApplicationManager(AbpModuleManager moduleManager, AbpModuleCollection modules)
        {
            _moduleManager = moduleManager;
            _modules = modules;
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public virtual void Initialize()
        {
            DependencyManager.AddConventionalRegisterer(new DomainServicesRegisterer()); //TODO: Remove somewhere else!

            var initializationContext = new AbpInitializationContext(_modules);
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
