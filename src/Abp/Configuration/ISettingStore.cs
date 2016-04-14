using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Configuration
{
    /// <summary>
    /// This interface is used to get/set settings from/to a data source (database).
    /// </summary>
    public interface ISettingStore
    {
        /// <summary>
        /// Gets a setting or null.
        /// </summary>
        /// <param name="tenantId">TenantId or null</param>
        /// <param name="userId">UserId or null</param>
        /// <param name="name">Name of the setting</param>
        /// <returns>Setting object</returns>
        Task<SettingInfo> GetSettingOrNullAsync(Guid? tenantId, Guid? userId, string name);

        /// <summary>
        /// Deletes a setting.
        /// </summary>
        /// <param name="setting">Setting to be deleted</param>
        Task DeleteAsync(SettingInfo setting);

        /// <summary>
        /// Adds a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        Task CreateAsync(SettingInfo setting);

        /// <summary>
        /// Update a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        Task UpdateAsync(SettingInfo setting);

        /// <summary>
        /// Gets a list of setting.
        /// </summary>
        /// <param name="tenantId">TenantId or null</param>
        /// <param name="userId">UserId or null</param>
        /// <returns>List of settings</returns>
        Task<List<SettingInfo>> GetAllListAsync(Guid? tenantId, Guid? userId);
    }
}