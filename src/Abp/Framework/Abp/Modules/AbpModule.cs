using System;
using Abp.Dependency;
using Abp.Startup.Configuration;

namespace Abp.Modules
{
    /// <summary>
    /// This class must be implemented by all module definition classes.
    /// </summary>
    public abstract class AbpModule
    {
        /// <summary>
        /// Gets a reference to the IOC manager.
        /// </summary>
        public IIocManager IocManager { get; internal set; }

        /// <summary>
        /// Gets a reference to the ABP configuration.
        /// </summary>
        public AbpConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Gets all depended modules for this module.
        /// </summary>
        /// <returns>List of depended modules.</returns>
        public virtual Type[] GetDependedModules()
        {
            return new Type[] {};
        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done before dependency registers.
        /// </summary>
        public virtual void PreInitialize()
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Register dependency installers and components.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done after dependency registers.
        /// </summary>
        public virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// This method is called when the application is being shutdown.
        /// </summary>
        public virtual void Shutdown()
        {
            
        }
    }
}
