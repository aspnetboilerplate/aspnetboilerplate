using System.Collections.Generic;

namespace Abp.Configuration
{
    /// <summary>
    /// Implement this interface to define settings for the application.
    /// </summary>
    public interface ISettingProvider
    {
        /// <summary>
        /// Gets all setting provided by this provider.
        /// </summary>
        /// <returns>Settings</returns>
        IEnumerable<Setting> GetSettings();
    }
}