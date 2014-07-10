using System.Collections.Generic;

namespace Abp.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class NullSettingStore : ISettingStore
    {
        public static NullSettingStore Instance { get { return SingletonInstance; } }
        private static readonly NullSettingStore SingletonInstance = new NullSettingStore();

        private NullSettingStore()
        {
            
        }

        public Setting GetSettingOrNull(int? tenantId, int? userId, string name)
        {
            return null;
        }

        public void Delete(Setting setting)
        {
            
        }

        public Setting Add(Setting settingValue)
        {
            return settingValue;
        }

        public List<Setting> GetAll(int? tenantId, int? userId)
        {
            return new List<Setting>();
        }
    }
}