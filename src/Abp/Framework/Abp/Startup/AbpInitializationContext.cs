using Abp.Dependency;
using Abp.Modules;
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
        public WindsorContainer IocContainer { get { return IocManager.Instance.IocContainer; } }

        private readonly AbpModuleCollection _modules;

        public AbpInitializationContext(AbpModuleCollection modules)
        {
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
    }
}