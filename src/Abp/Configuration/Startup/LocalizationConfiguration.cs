using System;
using System.Collections.Generic;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public class LocalizationConfiguration : ILocalizationConfiguration
    {
        /// <summary>
        /// Used to set languages available for this application.
        /// </summary>
        public List<LanguageInfo> Languages { get; private set; }

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        private readonly Lazy<ILocalizationManager> _localizationManager;

        internal LocalizationConfiguration(IIocResolver iocManager)
        {
            IsEnabled = true;
            _localizationManager = new Lazy<ILocalizationManager>(iocManager.Resolve<ILocalizationManager>);
            Languages = new List<LanguageInfo>();
        }
        
        /// <summary>
        /// Adds a localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        public void RegisterSource(ILocalizationSource source)
        {
            _localizationManager.Value.RegisterSource(source);
        }
    }
}