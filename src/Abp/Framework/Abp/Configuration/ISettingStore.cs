using System.Collections.Generic;

namespace Abp.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISettingStore
    {
        Setting GetSettingOrNull(int? tenantId, int? userId, string name);

        void Delete(Setting setting);

        Setting Add(Setting settingValue);

        List<Setting> GetAll(int? tenantId, int? userId);
    }
}