using System.Globalization;
using System.Web.Http;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Session;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Base class for all ApiControllers in web applications those use Abp system.
    /// </summary>
    public abstract class AbpApiController : ApiController
    {
        /// <summary>
        /// Gets current session information.
        /// </summary>
        public IAbpSession CurrentSession { get; set; }
        
        /// <summary>
        /// Reference to the permission manager.
        /// </summary>
        public IPermissionManager PermissionManager { get; set; }

        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this controller.
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName
        {
            get { return _localizationSource.Name; }
            set { _localizationSource = LocalizationHelper.GetSource(value); }
        }

        private ILocalizationSource _localizationSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpApiController()
        {
            CurrentSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            _localizationSource = NullLocalizationSource.Instance;
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return _localizationSource.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return _localizationSource.GetString(name, culture);
        }
    }
}
