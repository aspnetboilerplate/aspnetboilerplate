using System.Collections.Generic;

namespace Abp.Configuration
{
    /// <summary>
    /// Implement this interface to define settings for a module/application.
    /// </summary>
    public interface ISettingDefinitionProvider
    {
        /// <summary>
        /// Gets all setting definitions provided by this provider.
        /// </summary>
        /// <returns>List of settings.</returns>
        IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context);
    }
}