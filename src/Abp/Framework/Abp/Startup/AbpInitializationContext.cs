using Abp.Dependency;
using Abp.Modules;
using Abp.Startup.Configuration;
using Castle.Windsor;

namespace Abp.Startup
{
    /// <summary>
    /// This class is used by modules on initialization of the application.
    /// </summary>
    internal class AbpInitializationContext : IAbpInitializationContext
    {
        /// <summary>
        /// Gets a reference to the Ioc container. A shortcut for Abp.Dependency.IocManager.Instance.IocContainer.
        /// </summary>
        public IWindsorContainer IocContainer { get { return IocManager.IocContainer; } }

        public IocManager IocManager { get; private set; }

        private readonly AbpModuleCollection _modules;

        public AbpInitializationContext(IocManager iocManager, AbpModuleCollection modules)
        {
            IocManager = iocManager;
            _modules = modules;
        }

        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : AbpModule
        {
            return _modules.GetModule<TModule>();
        }

        public AbpConfiguration Configuration { get { return AbpConfiguration.Instance; } }
    }
}