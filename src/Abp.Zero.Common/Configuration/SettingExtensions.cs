namespace Abp.Configuration
{
    /// <summary>
    /// Implements methods to convert objects between SettingInfo and Setting classes.
    /// </summary>
    internal static class SettingExtensions
    {
        /// <summary>
        /// Creates new <see cref="Setting"/> object from given <see cref="SettingInfo"/> object.
        /// </summary>
        public static Setting ToSetting(this SettingInfo settingInfo)
        {
            return settingInfo == null
                ? null
                : new Setting(settingInfo.TenantId, settingInfo.UserId, settingInfo.Name, settingInfo.Value);
        }

        /// <summary>
        /// Creates new <see cref="SettingInfo"/> object from given <see cref="Setting"/> object.
        /// </summary>
        public static SettingInfo ToSettingInfo(this Setting setting)
        {
            return setting == null
                ? null
                : new SettingInfo(setting.TenantId, setting.UserId, setting.Name, setting.Value);
        }
    }
}