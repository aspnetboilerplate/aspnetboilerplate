using Abp.Collections;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used to configure setting system.
    /// </summary>
    public interface ISettingsConfiguration
    {
        /// <summary>
        /// List of authorization providers.
        /// </summary>
        ITypeList<SettingProvider> Providers { get; }
    }
}