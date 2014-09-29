using Abp.Configuration;
using Castle.Core.Logging;

namespace Abp.Domain.Services
{
    /// <summary>
    /// This class can be used as a base class for domain services. 
    /// </summary>
    public abstract class DomainService : IDomainService
    {
        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DomainService()
        {
            Logger = NullLogger.Instance;
        }
    }
}