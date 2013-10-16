using System.Globalization;
using System.Web.Mvc;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Views
{
    public abstract class AbpViewBase : AbpViewBase<dynamic>
    {

    }

    public abstract class AbpViewBase<TModel> : WebViewPage<TModel>
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
    }
}
