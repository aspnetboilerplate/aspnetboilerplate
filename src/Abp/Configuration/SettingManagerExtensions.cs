using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp.Threading;

namespace Abp.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ISettingManager"/>.
    /// </summary>
    public static class SettingManagerExtensions
    {
        /// <summary>
        /// Gets value of a setting in given type (<see cref="T"/>).
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static async Task<T> GetSettingValueAsync<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return (await settingManager.GetSettingValueAsync(name)).To<T>();
        }

        /// <summary>
        /// Gets value of a setting in given type (<see cref="T"/>).
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static T GetSettingValue<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return (settingManager.GetSettingValue(name)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        public static async Task<T> GetSettingValueForApplicationAsync<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return (await settingManager.GetSettingValueForApplicationAsync(name)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        public static T GetSettingValueForApplication<T>(this ISettingManager settingManager, string name)
            where T : struct
        {
            return (settingManager.GetSettingValueForApplication(name)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a tenant level.
        /// It gets the setting value, overwritten by given tenant.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <returns>Current value of the setting</returns>
        public static async Task<T> GetSettingValueForTenantAsync<T>(this ISettingManager settingManager, string name, int tenantId)
           where T : struct
        {
            return (await settingManager.GetSettingValueForTenantAsync(name, tenantId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a tenant level.
        /// It gets the setting value, overwritten by given tenant.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <returns>Current value of the setting</returns>
        public static T GetSettingValueForTenant<T>(this ISettingManager settingManager, string name, int tenantId)
           where T : struct
        {
            return (settingManager.GetSettingValueForTenant(name, tenantId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> GetSettingValueForUserAsync<T>(this ISettingManager settingManager, string name, int? tenantId, long userId)
           where T : struct
        {
            return (await settingManager.GetSettingValueForUserAsync(name, tenantId, userId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        public static T GetSettingValueForUser<T>(this ISettingManager settingManager, string name, int? tenantId, long userId)
           where T : struct
        {
            return (settingManager.GetSettingValueForUser(name, tenantId, userId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> GetSettingValueForUserAsync<T>(this ISettingManager settingManager, string name, UserIdentifier user)
           where T : struct
        {
            return (await settingManager.GetSettingValueForUserAsync(name, user)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        public static T GetSettingValueForUser<T>(this ISettingManager settingManager, string name, UserIdentifier user)
           where T : struct
        {
            return (settingManager.GetSettingValueForUser(name, user)).To<T>();
        }
    }
}