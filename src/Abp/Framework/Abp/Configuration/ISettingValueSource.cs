using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Configuration
{
    /// <summary>
    /// This is the main interface that must be implemented to be able to get/store values of settings.
    /// </summary>
    public interface ISettingValueSource : ISingletonDependency
    {
        /// <summary>
        /// Gets value of a setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting or null if not found</returns>
        string GetSettingValue(string name);

        /// <summary>
        /// Gets value of a setting.
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        T GetSettingValue<T>(string name);

        /// <summary>
        /// Gets current values of all settings.
        /// It gets all setting values, overwrited by application and the current user if exists.
        /// </summary>
        /// <returns>List of setting values</returns>
        IReadOnlyList<ISettingValue> GetAllSettingValues();

        /// <summary>
        /// Gets a list of all setting values specified for the application.
        /// It returns only settings those are explicitly set for the application.
        /// If a setting's default value is used, it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValues"/> method.
        /// </summary>
        /// <returns>List of setting values</returns>
        IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication();

        /// <summary>
        /// Gets a list of all setting values specified for a user.
        /// It returns only settings those are explicitly set for the user.
        /// If a setting's value is not set for the user (for example if user uses the default value), it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValues"/> method.
        /// </summary>
        /// <param name="userId">User to get settings</param>
        /// <returns>All settings of the user</returns>
        IReadOnlyList<ISettingValue> GetAllSettingValuesForUser(int userId);
    }
}
