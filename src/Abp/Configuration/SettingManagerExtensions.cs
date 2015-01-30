using System.Collections.Generic;
using Abp.Threading;

namespace Abp.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ISettingManager"/>.
    /// </summary>
    public static class SettingManagerExtensions
    {
        /// <summary>
        /// Gets current value of a setting.
        /// It gets the setting value, overwritten by application and the current user if exists.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting</returns>
        public static string GetSettingValue(this ISettingManager settingManager, string name)
        {
            return AsyncHelper.RunSync(() => settingManager.GetSettingValueAsync(name));
        }

        /// <summary>
        /// Gets value of a setting.
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static T GetSettingValue<T>(this ISettingManager settingManager, string name)
        {
            return AsyncHelper.RunSync(() => settingManager.GetSettingValueAsync<T>(name));
        }

        /// <summary>
        /// Gets current values of all settings.
        /// It gets all setting values, overwritten by application and the current user if exists.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <returns>List of setting values</returns>
        public static IReadOnlyList<ISettingValue> GetAllSettingValues(this ISettingManager settingManager)
        {
            return AsyncHelper.RunSync(settingManager.GetAllSettingValuesAsync);
        }

        /// <summary>
        /// Gets a list of all setting values specified for the application.
        /// It returns only settings those are explicitly set for the application.
        /// If a setting's default value is used, it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValues"/> method.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <returns>List of setting values</returns>
        public static IReadOnlyList<ISettingValue> GetAllSettingValuesForApplication(this ISettingManager settingManager)
        {
            return AsyncHelper.RunSync(settingManager.GetAllSettingValuesForApplicationAsync);
        }

        /// <summary>
        /// Gets a list of all setting values specified for a tenant.
        /// It returns only settings those are explicitly set for the tenant.
        /// If a setting's default value is used, it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValues"/> method.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="tenantId">Tenant to get settings</param>
        /// <returns>List of setting values</returns>
        public static IReadOnlyList<ISettingValue> GetAllSettingValuesForTenant(this ISettingManager settingManager, int tenantId)
        {
            return AsyncHelper.RunSync(() => settingManager.GetAllSettingValuesForTenantAsync(tenantId));
        }

        /// <summary>
        /// Gets a list of all setting values specified for a user.
        /// It returns only settings those are explicitly set for the user.
        /// If a setting's value is not set for the user (for example if user uses the default value), it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValues"/> method.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="userId">User to get settings</param>
        /// <returns>All settings of the user</returns>
        public static IReadOnlyList<ISettingValue> GetAllSettingValuesForTenant(this ISettingManager settingManager, long userId)
        {
            return AsyncHelper.RunSync(() => settingManager.GetAllSettingValuesForUserAsync(userId));
        }

        /// <summary>
        /// Changes setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public static void ChangeSettingForApplication(this ISettingManager settingManager, string name, string value)
        {
            AsyncHelper.RunSync(() => settingManager.ChangeSettingForApplicationAsync(name, value));
        }

        /// <summary>
        /// Changes setting for a Tenant.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="tenantId">TenantId</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public static void ChangeSettingForTenant(this ISettingManager settingManager, int tenantId, string name, string value)
        {
            AsyncHelper.RunSync(() => settingManager.ChangeSettingForTenantAsync(tenantId, name, value));
        }

        /// <summary>
        /// Changes setting for a user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="userId">UserId</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public static void ChangeSettingForUser(this ISettingManager settingManager, long userId, string name, string value)
        {
            AsyncHelper.RunSync(() => settingManager.ChangeSettingForUserAsync(userId, name, value));
        }
    }
}