using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Abp.Logging;

namespace Abp.Configuration
{
    /// <summary>
    ///     Implements default behavior for ISettingStore.
    ///     Only <see cref="GetSettingOrNullAsync" /> method is implemented and it gets setting's value
    ///     from application's configuration file if exists, or returns null if not.
    /// </summary>
    public class DefaultConfigSettingStore : ISettingStore
    {
        private DefaultConfigSettingStore()
        {
        }

        /// <summary>
        ///     Gets singleton instance.
        /// </summary>
        public static DefaultConfigSettingStore Instance { get; } = new DefaultConfigSettingStore();

        public Task<SettingInfo> GetSettingOrNullAsync(Guid? tenantId, Guid? userId, string name)
        {
            var value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                return Task.FromResult<SettingInfo>(null);
            }

            return Task.FromResult(new SettingInfo(tenantId, userId, name, value));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support DeleteAsync.");
        }

        /// <inheritdoc />
        public async Task CreateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support CreateAsync.");
        }

        /// <inheritdoc />
        public async Task UpdateAsync(SettingInfo setting)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support UpdateAsync.");
        }

        /// <inheritdoc />
        public Task<List<SettingInfo>> GetAllListAsync(Guid? tenantId, Guid? userId)
        {
            LogHelper.Logger.Warn(
                "ISettingStore is not implemented, using DefaultConfigSettingStore which does not support GetAllListAsync.");
            return Task.FromResult(new List<SettingInfo>());
        }
    }
}