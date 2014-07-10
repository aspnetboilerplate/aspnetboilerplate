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

        public Setting GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            return null;
        }

        public void Delete(Setting setting)
        {
            
        }

        public void Add(Setting setting)
        {

        }

        public List<Setting> GetAll(int? tenantId, long? userId)
        {
            return new List<Setting>();
        }
    }
}