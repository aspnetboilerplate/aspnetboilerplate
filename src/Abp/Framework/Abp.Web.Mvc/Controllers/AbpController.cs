using System.Globalization;
using System.Text;
using System.Web.Mvc;
using Abp.Localization;
using Abp.Web.Mvc.Controllers.Results;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// Base class for all MVC Controllers in Abp system.
    /// </summary>
    public abstract class AbpController : Controller
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

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonCamelCaseResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}