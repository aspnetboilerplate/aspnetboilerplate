using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Configuration
{
    /// <summary>
    /// This is the main interface that must be implemented to be able to load/change values of settings.
    /// </summary>
    public interface ISettingManager
    {
        /// <summary>
        /// Gets current value of a setting.
        /// It gets the setting value, overwritten by application, current tenant and current user if exists.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting</returns>
        Task<string> GetSettingValueAsync(string name);

        /// <summary>
        /// Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        Task<string> GetSettingValueForApplicationAsync(string name);

        /// <summary>
        /// Gets current value of a setting for the application level.
        /// If fallbackToDefault is false, it just gets value from application and returns null if application has not defined a value for the setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="fallbackToDefault"></param>
        /// <returns>Current value of the setting for the application</returns>
        Task<string> GetSettingValueForApplicationAsync(string name, bool fallbackToDefault);

        /// <summary>
        /// Gets current value of a setting for a tenant level.
        /// It gets the setting value, overwritten by given tenant.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <returns>Current value of the setting</returns>
        Task<string> GetSettingValueForTenantAsync(string name, int tenantId);

        /// <summary>
        /// Gets current value of a setting for a tenant level.
        /// It gets the setting value, overwritten by given tenant if fallbackToDefault is true.
        /// If fallbackToDefault is false, it just gets value from tenant and returns null if tenant has not defined a value for the setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="fallbackToDefault"></param>
        /// <returns>Current value of the setting</returns>
        Task<string> GetSettingValueForTenantAsync(string name, int tenantId, bool fallbackToDefault);

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        Task<string> GetSettingValueForUserAsync(string name, int? tenantId, long userId);

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user if fallbackToDefault is true.
        /// If fallbackToDefault is false, it just gets value from user and returns null if user has not defined a value for the setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <param name="fallbackToDefault"></param>
        /// <returns>Current value of the setting for the user</returns>
        Task<string> GetSettingValueForUserAsync(string name, int? tenantId, long userId, bool fallbackToDefault);

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        Task<string> GetSettingValueForUserAsync(string name, UserIdentifier user);

        /// <summary>
        /// Gets current values of all settings.
        /// It gets all setting values, overwritten by application, current tenant (if exists) and the current user (if exists).
        /// </summary>
        /// <returns>List of setting values</returns>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync();

        /// <summary>
        /// Gets current values of all settings.
        /// It gets default values of all settings then overwrites by given scopes.
        /// </summary>
        /// <param name="scopes">One or more scope to overwrite</param>
        /// <returns>List of setting values</returns>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesAsync(SettingScopes scopes);

        /// <summary>
        /// Gets a list of all setting values specified for the application.
        /// It returns only settings those are explicitly set for the application.
        /// If a setting's default value is used, it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValuesAsync()"/> method.
        /// </summary>
        /// <returns>List of setting values</returns>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForApplicationAsync();

        /// <summary>
        /// Gets a list of all setting values specified for a tenant.
        /// It returns only settings those are explicitly set for the tenant.
        /// If a setting's default value is used, it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValuesAsync()"/> method.
        /// </summary>
        /// <param name="tenantId">Tenant to get settings</param>
        /// <returns>List of setting values</returns>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForTenantAsync(int tenantId);

        /// <summary>
        /// Gets a list of all setting values specified for a user.
        /// It returns only settings those are explicitly set for the user.
        /// If a setting's value is not set for the user (for example if user uses the default value), it's not included the result list.
        /// If you want to get current values of all settings, use <see cref="GetAllSettingValuesAsync()"/> method.
        /// </summary>
        /// <param name="user">User to get settings</param>
        /// <returns>All settings of the user</returns>
        Task<IReadOnlyList<ISettingValue>> GetAllSettingValuesForUserAsync(UserIdentifier user);

        /// <summary>
        /// Changes setting for the application level.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        Task ChangeSettingForApplicationAsync(string name, string value);

        /// <summary>
        /// Changes setting for a Tenant.
        /// </summary>
        /// <param name="tenantId">TenantId</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        Task ChangeSettingForTenantAsync(int tenantId, string name, string value);

        /// <summary>
        /// Changes setting for a user.
        /// </summary>
        /// <param name="user">UserId</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value);
    }
}
