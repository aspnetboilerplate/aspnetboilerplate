using System.Globalization;
using System.Web.Http;
using Abp.Exceptions;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Base class for all ApiControllers in web applications those use Abp system.
    /// </summary>
    public abstract class AbpApiController : ApiController
    {
        /// <summary>
        /// Gets/sets name of the localization source that is used in this controller.
        /// It's used in <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected string L(string name)
        {
            CheckForLocalizationSourceName();

            return LocalizationHelper.GetString(LocalizationSourceName, name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected string L(string name, CultureInfo culture)
        {
            CheckForLocalizationSourceName();

            return LocalizationHelper.GetString(LocalizationSourceName, name, culture);
        }

        private void CheckForLocalizationSourceName()
        {
            if (LocalizationSourceName == null)
            {
                throw new AbpException("You must set LocalizationSourceName property before use L methods.");
            }
        }
    }
}
