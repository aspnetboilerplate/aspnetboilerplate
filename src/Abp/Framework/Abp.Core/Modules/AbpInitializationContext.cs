using Castle.Windsor;

namespace Abp.Modules
{
    /// <summary>
    /// Defines properties and methods those can be used while initialization progress.
    /// </summary>
    public interface IAbpInitializationContext
    {
        /// <summary>
        /// Reference to the IOC container.
        /// </summary>
        WindsorContainer IocContainer { get; }

        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Type of the module</typeparam>
        /// <returns>The module instance</returns>
        TModule GetModule<TModule>() where TModule : AbpModule;
    }
}