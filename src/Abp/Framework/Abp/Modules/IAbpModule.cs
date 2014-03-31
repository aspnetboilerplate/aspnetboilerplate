using System;
using Abp.Startup;

namespace Abp.Modules
{
    /// <summary>
    /// A module must implement this interface generally by inheriting <see cref="AbpModule"/> class.
    /// </summary>
    public interface IAbpModule
    {
        /// <summary>
        /// Gets all depended modules for this module.
        /// </summary>
        /// <returns>List of depended modules.</returns>
        Type[] GetDependedModules();

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done before dependency registers.
        /// </summary>
        /// <param name="initializationContext"> </param>
        void PreInitialize(IAbpInitializationContext initializationContext);

        /// <summary>
        /// What can be done in this method:
        /// - Register dependency installers and components.
        /// </summary>
        /// <param name="initializationContext"> </param>
        void Initialize(IAbpInitializationContext initializationContext);

        /// <summary>
        /// What can be done in this method:
        /// - Make things those must be done after dependency registers.
        /// </summary>
        /// <param name="initializationContext"> </param>
        void PostInitialize(IAbpInitializationContext initializationContext);

        /// <summary>
        /// This method is called when the system is being shutdown.
        /// </summary>
        void Shutdown();
    }
}