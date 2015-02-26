using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Abp.Web.Mvc.Views
{
    /// <summary>
    /// Base class for all views in Abp system.
    /// </summary>
    /// <typeparam name="TModel">Type of the View Model</typeparam>
    public abstract class AbpWebViewPage<TModel> : WebViewPage<TModel>
    {
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

                appPath = appPath.EnsureEndsWith('/');

                return appPath;
            }
        }

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
        
        private IPermissionChecker PermissionChecker { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbpWebViewPage()
        {
            _localizationSource = NullLocalizationSource.Instance;
            PermissionChecker = IocManager.Instance.Resolve<IPermissionChecker>();
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
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        public string L(string name, params object[] args)
        {
            return _localizationSource.GetString(name, args);
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

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        public string L(string name, CultureInfo culture, params object[] args)
        {
            return _localizationSource.GetString(name, culture, args);
        }
        
        /// <summary>
        /// Check if user has any permission on the list
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        /// <returns>True if user has any of permissions</returns>
        protected virtual bool hasAnyOfPermissions(params string[] permissions)
        {
            foreach (var permissionName in permissions)
            {
                if (PermissionChecker.IsGranted(permissionName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if user has all permissions on the list
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        /// <returns>True if user has all permissions</returns>
        protected virtual bool hasAllOfPermissions(params string[] permissions)
        {
            foreach (var permissionName in permissions)
            {
                if (!PermissionChecker.IsGranted(permissionName))
                    return false;
            }
            return true;
        }
    }
}
