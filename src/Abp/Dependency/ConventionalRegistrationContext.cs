using System.Reflection;

namespace Abp.Dependency
{
    /// <summary>
    ///     This class is used to pass needed objects on conventional registration process.
    /// </summary>
    internal class ConventionalRegistrationContext : IConventionalRegistrationContext
    {
        internal ConventionalRegistrationContext(Assembly assembly, IIocManager iocManager,
            ConventionalRegistrationConfig config)
        {
            Assembly = assembly;
            IocManager = iocManager;
            Config = config;
        }

        /// <summary>
        ///     Gets the registering Assembly.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        ///     Reference to the IOC Container to register types.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     Registration configuration.
        /// </summary>
        public ConventionalRegistrationConfig Config { get; }
    }
}