using System;
using Abp.Startup;
using Abp.Startup.Configuration;

namespace Abp.Modules
{
    /// <summary>
    /// This class must be implemented by all module definition classes.
    /// </summary>
    public abstract class AbpModule
    {
        /// <summary>
        /// Gets all depended modules for this module.
        /// </summary>
        /// <returns>List of depended modules.</returns>
        public virtual Type[] GetDependedModules()
        {
            return new Type[] {};
        }

        /// <summary>
        /// This method can be used to configure ABP system and depended modules.
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        public virtual void Configure(AbpConfiguration configuration)
        {
            
        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done before dependency registers.
        /// </summary>
        /// <param name="initializationContext">Initialization context</param>
        public virtual void PreInitialize(IAbpInitializationContext initializationContext)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Register dependency installers and components.
        /// </summary>
        /// <param name="initializationContext">Initialization context</param>
        public virtual void Initialize(IAbpInitializationContext initializationContext)
        {

        }

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done after dependency registers.
        /// </summary>
        /// <param name="initializationContext">Initialization context</param>
        public virtual void PostInitialize(IAbpInitializationContext initializationContext)
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
