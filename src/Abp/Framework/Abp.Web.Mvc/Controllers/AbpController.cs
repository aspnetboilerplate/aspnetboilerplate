using System.Globalization;
using System.Text;
using System.Web.Mvc;
using Abp.Localization;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers.Results;
using Abp.Web.Mvc.Models;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// Base class for all MVC Controllers in Abp system.
    /// </summary>
    public abstract class AbpController : Controller
    {
        public string LocalizationSourceName { get; set; }

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public virtual ILogger Logger { get; set; }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// Shortcut for LocalizationManager.GetString.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationHelper.GetString(LocalizationSourceName, name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// Shortcut for LocalizationManager.GetString.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationHelper.GetString(LocalizationSourceName, name, culture);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (!(data is AbpAjaxResponse))
            {
                data = new AbpMvcAjaxResponse(data);
            }

            return new AbpJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}