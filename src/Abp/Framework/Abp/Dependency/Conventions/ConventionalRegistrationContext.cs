using System.Reflection;
using Castle.Windsor;

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
        public IWindsorContainer IocContainer { get; private set; }

        /// <summary>
        /// Registration configuration.
        /// </summary>
        public ConventionalRegistrationConfig Config { get; private set; }

        internal ConventionalRegistrationContext(Assembly assembly, IWindsorContainer iocContainer, ConventionalRegistrationConfig config)
        {
            Assembly = assembly;
            IocContainer = iocContainer;
            Config = config;
        }
    }
}