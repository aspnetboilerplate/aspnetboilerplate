using System;
using Abp.Dependency;
using Abp.Threading;

namespace Abp.Configuration
{
    /// <summary>
    /// This class is used to simplify getting settings from anywhere.
    /// </summary>
    [Obsolete("Don't use this class wherever possible since it breaks testability of the code and may be removed in the future.")]
    public static class SettingHelper
    {
        private static readonly Lazy<ISettingManager> SettingManager;

        static SettingHelper()
        {
            SettingManager = new Lazy<ISettingManager>(IocManager.Instance.Resolve<ISettingManager>);
        }

        /// <summary>
        /// Gets current value of a setting.
        /// It gets the setting value, overwrited by application and the current user if exists.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting</returns>
        public static string GetSettingValue(string name)
        {
            return AsyncHelper.RunSync(() => SettingManager.Value.GetSettingValueAsync(name));
        }

        /// <summary>
        /// Gets value of a setting.
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static T GetSettingValue<T>(string name)
            where T : struct
        {
            return AsyncHelper.RunSync(() => SettingManager.Value.GetSettingValueAsync<T>(name));
        }
    }
}
