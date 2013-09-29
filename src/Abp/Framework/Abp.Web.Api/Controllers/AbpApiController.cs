using System.Globalization;
using System.Web.Http;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Base class for all ApiControllers in Abp system.
    /// </summary>
    public abstract class AbpApiController : ApiController
    {
        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager
        {
            get { return _localizationManager; }
            set { _localizationManager = value; }
        }
        private ILocalizationManager _localizationManager = NullLocalizationManager.Instance;

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// Shortcut for LocalizationManager.GetString.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected string L(string name)
        {
            return LocalizationManager.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified language.
        /// Shortcut for LocalizationManager.GetString.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="languageCode">Language</param>
        /// <returns>Localized string</returns>
        protected string L(string name, string languageCode)
        {
            return LocalizationManager.GetString(name, languageCode);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// Shortcut for LocalizationManager.GetString.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected string L(string name, CultureInfo culture)
        {
            return LocalizationManager.GetString(name, culture);
        }
    }
}
