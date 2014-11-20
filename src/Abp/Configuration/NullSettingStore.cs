using System.Collections.Generic;

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

        /// <inheritdoc/>
        public SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            return null;
        }

        /// <inheritdoc/>
        public void Delete(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public void Create(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public void Update(SettingInfo setting)
        {
        }

        /// <inheritdoc/>
        public List<SettingInfo> GetAll(int? tenantId, long? userId)
        {
            return new List<SettingInfo>();
        }
    }
}