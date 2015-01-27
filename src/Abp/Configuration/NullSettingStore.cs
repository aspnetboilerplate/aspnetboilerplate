using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Configuration
{
    /// <summary>
    /// Implements null pattern for ISettingStore.
    /// </summary>
    public class NullSettingStore : ISettingStore
    {
        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static NullSettingStore Instance { get { return SingletonInstance; } }
        private static readonly NullSettingStore SingletonInstance = new NullSettingStore();

        private NullSettingStore()
        {
        }

        public Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            return Task.FromResult<SettingInfo>(null);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public async Task CreateAsync(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            return Task.FromResult(new List<SettingInfo>());
        }
    }
}