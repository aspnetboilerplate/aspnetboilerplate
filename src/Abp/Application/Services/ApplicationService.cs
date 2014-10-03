using System.Globalization;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Localization;
using Abp.Runtime.Session;
using Castle.Core.Logging;

namespace Abp.Application.Services
{
    /// <summary>
    /// This class can be used as a base class for application services. 
    /// </summary>
    public abstract class ApplicationService : IApplicationService
    {
        /// <summary>
        /// Gets current session informations.
        /// </summary>
        public IAbpSession CurrentSession { protected get; set; }

        /// <summary>
        /// Reference to the permission manager.
        /// </summary>
        public IPermissionManager PermissionManager { protected get; set; }

        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { protected get; set; }
        
        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { protected get; set; }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this controller.
        /// It's used in <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ApplicationService()
        {
            CurrentSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationHelper.GetString(LocalizationSourceName, name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationHelper.GetString(LocalizationSourceName, name, culture);
        }
    }
}
