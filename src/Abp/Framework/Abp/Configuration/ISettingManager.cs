using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Configuration
{
    /// <summary>
    /// Defines setting manager
    /// </summary>
    public interface ISettingManager : ISingletonDependency
    {
        /// <summary>
        /// Gets the <see cref="Setting"/> object with given unique name.
        /// Throws exception if can not find the setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>The <see cref="Setting"/> object.</returns>
        Setting GetSetting(string name);

        /// <summary>
        /// Gets a list of all settings.
        /// </summary>
        /// <returns>All settings.</returns>
        IReadOnlyList<Setting> GetAllSettings();
    }
}
