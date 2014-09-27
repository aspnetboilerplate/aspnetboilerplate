using System;
using Abp.Dependency;
using Abp.Localization.Sources;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public class AbpLocalizationConfiguration
    {
        private readonly Lazy<ILocalizationSourceManager> _localizationSourceManager;

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        internal AbpLocalizationConfiguration(IIocResolver iocManager)
        {
            IsEnabled = true;
            _localizationSourceManager = new Lazy<ILocalizationSourceManager>(iocManager.Resolve<ILocalizationSourceManager>);
        }

        /// <summary>
        /// Adds a localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        public void RegisterSource(ILocalizationSource source)
        {
            _localizationSourceManager.Value.RegisterSource(source);
        }
    }
}