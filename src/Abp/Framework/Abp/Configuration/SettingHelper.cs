using Abp.Dependency;

namespace Abp.Configuration
{
    /// <summary>
    /// This class is used to simplify getting settings from anywhere.
    /// </summary>
    public static class SettingHelper
    {
        private static readonly ISettingManager SettingManager;

        static SettingHelper()
        {
            SettingManager = IocHelper.Resolve<ISettingManager>();
        }

        /// <summary>
        /// Gets current value of a setting.
        /// It gets the setting value, overwrited by application and the current user if exists.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting</returns>
        public static string GetSettingValue(string name)
        {
            return SettingManager.GetSettingValue(name);
        }

        /// <summary>
        /// Gets value of a setting.
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static T GetSettingValue<T>(string name)
        {
            return SettingManager.GetSettingValue<T>(name);
        }
    }
}
