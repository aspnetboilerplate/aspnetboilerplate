using System;
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
        private readonly Lazy<ILocalizationManager> _localizationManager;

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        internal LocalizationConfiguration(IIocResolver iocManager)
        {
            IsEnabled = true;
            _localizationManager = new Lazy<ILocalizationManager>(iocManager.Resolve<ILocalizationManager>);
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