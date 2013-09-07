namespace Abp.Modules
{
    public interface IAbpModule
    {
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
    }
}