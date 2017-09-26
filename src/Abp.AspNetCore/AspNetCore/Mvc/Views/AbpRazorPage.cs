using System.Globalization;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Localization;
using Abp.Localization.Sources;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Abp.AspNetCore.Mvc.Views
{
    /// <summary>
    /// Base class for all views in Abp system.
    /// </summary>
    /// <typeparam name="TModel">Type of the View Model</typeparam>
    public abstract class AbpRazorPage<TModel> : RazorPage<TModel>
    {
        /// <summary>
        /// Gets the root path of the application.
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                //TODO: Implement this
                return "/";
                //var appPath = HttpContext.Current.Request.ApplicationPath;
                //if (appPath == null)
                //{
                //    return "/";
                //}

                //appPath = appPath.EnsureEndsWith('/');

                //return appPath;
            }
        }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        [RazorInject]
        public ILocalizationManager LocalizationManager { get; set; }

        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        [RazorInject]
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        [RazorInject]
        public IPermissionChecker PermissionChecker { get; set; }

        /// <summary>
        /// Reference to the feature checker.
        /// </summary>
        [RazorInject]
        public IFeatureChecker FeatureChecker { get; set; }

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
        protected AbpRazorPage()
        {
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
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
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
            return LocalizationManager.GetSource(sourceName).GetString(name);
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
            return LocalizationManager.GetSource(sourceName).GetString(name, args);
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
            return LocalizationManager.GetSource(sourceName).GetString(name, culture);
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
            return LocalizationManager.GetSource(sourceName).GetString(name, culture, args);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected virtual bool IsGranted(string permissionName)
        {
            return PermissionChecker.IsGranted(permissionName);
        }

        /// <summary>
        /// Determines whether is given feature enabled.
        /// </summary>
        /// <param name="featureName">Name of the feature.</param>
        /// <returns>True, if enabled; False if not.</returns>
        protected virtual bool IsFeatureEnabled(string featureName)
        {
            return FeatureChecker.IsEnabled(featureName);
        }

        /// <summary>
        /// Gets current value of a feature.
        /// </summary>
        /// <param name="featureName">Feature name</param>
        /// <returns>Value of the feature</returns>
        protected virtual string GetFeatureValue(string featureName)
        {
            return FeatureChecker.GetValue(featureName);
        }
    }
}
