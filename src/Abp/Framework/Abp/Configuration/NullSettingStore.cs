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

        public SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            return null;
        }

        public void Delete(SettingInfo setting)
        {
            
        }

        public void Create(SettingInfo setting)
        {

        }

        public void Update(SettingInfo setting)
        {
            
        }

        public List<SettingInfo> GetAll(int? tenantId, long? userId)
        {
            return new List<SettingInfo>();
        }
    }
}