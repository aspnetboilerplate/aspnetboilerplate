using System;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup.Configuration;
using Castle.Windsor;

namespace Abp.Startup
{
    /// <summary>
    /// Defines properties and methods those can be used while initialization progress.
    /// </summary>
    public interface IAbpInitializationContext
    {
        /// <summary>
        /// Gets a reference to the Ioc container. A shortcut for Abp.Dependency.IocManager.Instance.IocContainer.
        /// </summary>
        [Obsolete("Use IocManager. This property will be removed in future releases.")]
        IWindsorContainer IocContainer { get; }

        /// <summary>
        /// Gets IOC Manager to perform dependency injection works.
        /// </summary>
        IIocManager IocManager { get; }

        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Type of the module</typeparam>
        /// <returns>The module instance</returns>
        TModule GetModule<TModule>() where TModule : AbpModule;

        /// <summary>
        /// Used to configure ABP and modules..
        /// </summary>
        AbpConfiguration Configuration { get; }
    }
}