using System;

namespace Abp.Configuration
{
    /// <summary>
    /// Represents a setting information.
    /// </summary>
    [Serializable]
    public class SettingInfo
    {
        /// <summary>
        /// TenantId for this setting.
        /// TenantId is null if this setting is not Tenant level.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// UserId for this setting.
        /// UserId is null if this setting is not user level.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Unique name of the setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="SettingInfo"/> object.
        /// </summary>
        public SettingInfo()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="SettingInfo"/> object.
        /// </summary>
        /// <param name="tenantId">TenantId for this setting. TenantId is null if this setting is not Tenant level.</param>
        /// <param name="userId">UserId for this setting. UserId is null if this setting is not user level.</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public SettingInfo(int? tenantId, long? userId, string name, string value)
        {
            TenantId = tenantId;
            UserId = userId;
            Name = name;
            Value = value;
        }
    }
}