using System;

namespace Abp.Configuration
{
    /// <summary>
    /// Defines scope of a setting.
    /// </summary>
    [Flags]
    public enum SettingScopes
    {
        /// <summary>
        /// Represents a setting that can be configured/changed for the application.
        /// </summary>
        Application,

        /// <summary>
        /// Represents a setting that can be configured/changed for every user.
        /// </summary>
        User
    }
}