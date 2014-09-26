using System;
using Abp.Dependency;
using Abp.Startup;

namespace Abp.Modules
{
    /// <summary>
    /// This class must be implemented by all module definition classes.
    /// </summary>
    public abstract class AbpModule
    {
        ///// <summary>
        ///// Gets a reference to the IOC manager.
        ///// </summary>
        //public IIocManager IocManager { get; set; }

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
        /// <param name="context">Initialization context</param>
        public virtual void PreInitialize(IAbpInitializationContext context)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Register dependency installers and components.
        /// </summary>
        /// <param name="context">Initialization context</param>
        public virtual void Initialize(IAbpInitializationContext context)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done after dependency registers.
        /// </summary>
        /// <param name="context">Initialization context</param>
        public virtual void PostInitialize(IAbpInitializationContext context)
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
