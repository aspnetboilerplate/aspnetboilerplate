using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Mvc.Views
{
    /// <summary>
    /// Abp系统中所有视图的基类。
    /// </summary>
    /// <typeparam name="TModel">视图模型的类型</typeparam>
    public abstract class AbpWebViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// 获取应用程序的根路径。
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
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// 获取/设置此控制器中使用的本地化源的名称。
        /// 必须设置为使用<see cref ="L(string)"/>和<see cref ="L(string,CultureInfo)"/>方法。
        /// </summary>
        protected string LocalizationSourceName
        {
            get { return _localizationSource.Name; }
            set { _localizationSource = LocalizationHelper.GetSource(value); }
        }
        private ILocalizationSource _localizationSource;

        /// <summary>
        /// 构造函数。
        /// </summary>
        protected AbpWebViewPage()
        {
            _localizationSource = NullLocalizationSource.Instance;
            SettingManager = SingletonDependency<ISettingManager>.Instance;
        }

        /// <summary>
        /// 获取给定键名称和当前语言的本地化字符串。
        /// </summary>
        /// <param name="name">主要名称</param>
        /// <returns>本地化字符串</returns>
        protected virtual string L(string name)
        {
            return _localizationSource.GetString(name);
        }

        /// <summary>
        /// 为格式化字符串获取给定键名称和当前语言的本地化字符串。
        /// </summary>
        /// <param name="name">主要名称</param>
        /// <param name="args">格式参数</param>
        /// <returns>本地化字符串</returns>
        protected virtual string L(string name, params object[] args)
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
        protected string L(string name, CultureInfo culture, params object[] args)
        {
            return _localizationSource.GetString(name, culture, args);
        }

        /// <summary>
        /// Gets localized string from given source for given key name and current language.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string Ls(string sourceName, string name)
        {
            return LocalizationHelper.GetSource(sourceName).GetString(name);
        }

        /// <summary>
        /// Gets localized string from given source  for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string Ls(string sourceName, string name, params object[] args)
        {
            return LocalizationHelper.GetSource(sourceName).GetString(name, args);
        }

        /// <summary>
        /// Gets localized string from given source  for given key name and specified culture information.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string Ls(string sourceName, string name, CultureInfo culture)
        {
            return LocalizationHelper.GetSource(sourceName).GetString(name, culture);
        }

        /// <summary>
        /// Gets localized string from given source  for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string Ls(string sourceName, string name, CultureInfo culture, params object[] args)
        {
            return LocalizationHelper.GetSource(sourceName).GetString(name, culture, args);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected virtual bool IsGranted(string permissionName)
        {
            return SingletonDependency<IPermissionChecker>.Instance.IsGranted(permissionName);
        }

        /// <summary>
        /// Determines whether is given feature enabled.
        /// </summary>
        /// <param name="featureName">Name of the feature.</param>
        /// <returns>True, if enabled; False if not.</returns>
        protected virtual bool IsFeatureEnabled(string featureName)
        {
            return SingletonDependency<IFeatureChecker>.Instance.IsEnabled(featureName);
        }

        /// <summary>
        /// Gets current value of a feature.
        /// </summary>
        /// <param name="featureName">Feature name</param>
        /// <returns>Value of the feature</returns>
        protected virtual string GetFeatureValue(string featureName)
        {
            return SingletonDependency<IFeatureChecker>.Instance.GetValue(featureName);
        }

        protected virtual void SetAntiForgeryCookie()
        {
            SingletonDependency<IAbpAntiForgeryManager>.Instance.SetCookie(Context);
        }
    }
}
