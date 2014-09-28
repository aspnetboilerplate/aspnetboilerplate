using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Abp.Localization;

namespace Abp.Web.Mvc.Views
{
    /// <summary>
    /// Base class for all views in Abp system.
    /// </summary>
    /// <typeparam name="TModel">Type of the View Model</typeparam>
    public abstract class AbpWebViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// Gets/sets name of the localization source that is used in this controller.
        /// It's used in <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        public string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets the root path of the application.
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                var appPath = HttpContext.Current.Request.ApplicationPath;
                if (appPath == null)
                {
                    return "/";
                }

                if (!appPath.EndsWith("/"))
                {
                    appPath += "/";
                }

                return appPath;
            }
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
