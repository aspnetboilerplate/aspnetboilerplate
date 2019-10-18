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
        Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name);

        /// <summary>
        /// Gets a setting or null.
        /// </summary>
        /// <param name="tenantId">TenantId or null</param>
        /// <param name="userId">UserId or null</param>
        /// <param name="name">Name of the setting</param>
        /// <returns>Setting object</returns>
        SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name);

        /// <summary>
        /// Deletes a setting.
        /// </summary>
        /// <param name="setting">Setting to be deleted</param>
        Task DeleteAsync(SettingInfo setting);

        /// <summary>
        /// Deletes a setting.
        /// </summary>
        /// <param name="setting">Setting to be deleted</param>
        void Delete(SettingInfo setting);

        /// <summary>
        /// Adds a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        Task CreateAsync(SettingInfo setting);

        /// <summary>
        /// Adds a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        void Create(SettingInfo setting);

        /// <summary>
        /// Update a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        Task UpdateAsync(SettingInfo setting);

        /// <summary>
        /// Update a setting.
        /// </summary>
        /// <param name="setting">Setting to add</param>
        void Update(SettingInfo setting);

        /// <summary>
        /// Gets a list of setting.
        /// </summary>
        /// <param name="tenantId">TenantId or null</param>
        /// <param name="userId">UserId or null</param>
        /// <returns>List of settings</returns>
        Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId);

        /// <summary>
        /// Gets a list of setting.
        /// </summary>
        /// <param name="tenantId">TenantId or null</param>
        /// <param name="userId">UserId or null</param>
        /// <returns>List of settings</returns>
        List<SettingInfo> GetAllList(int? tenantId, long? userId);
    }
}