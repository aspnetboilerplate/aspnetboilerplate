using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingStore : ISettingStore, ITransientDependency
    {
        private readonly ISettingRepository _settingRepository;

        public SettingStore(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public Setting GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            return _settingRepository.FirstOrDefault(s => s.TenantId == tenantId && s.UserId == userId && s.Name == name);
        }

        public void Delete(Setting setting)
        {
            _settingRepository.Delete(setting);
        }

        public void Add(Setting setting)
        {
            _settingRepository.Insert(setting);
        }

        public List<Setting> GetAll(int? tenantId, long? userId)
        {
            return _settingRepository.GetAllList(s => s.TenantId == tenantId && s.UserId == userId);
        }
    }
}