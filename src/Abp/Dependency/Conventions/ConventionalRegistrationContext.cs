using System.Reflection;

namespace Abp.Dependency.Conventions
{
    /// <summary>
    /// This class is used to pass needed objects on conventional registration process.
    /// </summary>
    public class ConventionalRegistrationContext
    {
        /// <summary>
        /// Gets the registring Assembly.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Reference to the IOC Container to register types.
        /// </summary>
        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// Registration configuration.
        /// </summary>
        public ConventionalRegistrationConfig Config { get; private set; }

        internal ConventionalRegistrationContext(Assembly assembly, IIocManager iocManager, ConventionalRegistrationConfig config)
        {
            Assembly = assembly;
            IocManager = iocManager;
            Config = config;
        }
    }
}