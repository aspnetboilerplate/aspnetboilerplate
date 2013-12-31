using Castle.Windsor;

namespace Abp.Modules
{
    /// <summary>
    /// Defines properties and methods those can be used while initialization progress.
    /// </summary>
    public interface IAbpInitializationContext
    {
        /// <summary>
        /// Gets a reference to the Ioc container. A shortcut for Abp.Dependency.IocManager.Instance.IocContainer.
        /// </summary>
        WindsorContainer IocContainer { get; }

        string ApplicationDirectory { get; }

        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Type of the module</typeparam>
        /// <returns>The module instance</returns>
        TModule GetModule<TModule>() where TModule : AbpModule;
    }
}