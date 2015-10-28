using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingStore"/>.
    /// </summary>
    public class SettingStore : ISettingStore, ITransientDependency
    {
        private readonly IRepository<Setting, long> _settingRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingStore(IRepository<Setting, long> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public virtual async Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            var settings = await _settingRepository.GetAllListAsync(s => s.TenantId == tenantId && s.UserId == userId);
            return settings.Select(s => s.ToSettingInfo()).ToList();
        }

        public virtual async Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            var setting = await _settingRepository.FirstOrDefaultAsync(s => s.TenantId == tenantId && s.UserId == userId && s.Name == name);
            return setting.ToSettingInfo();
        }

        public virtual async Task DeleteAsync(SettingInfo settingInfo)
        {
            await _settingRepository.DeleteAsync(
                s => s.TenantId == settingInfo.TenantId && s.UserId == settingInfo.UserId && s.Name == settingInfo.Name
                );
        }

        public virtual async Task CreateAsync(SettingInfo settingInfo)
        {
            await _settingRepository.InsertAsync(settingInfo.ToSetting());
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(SettingInfo settingInfo)
        {
            var setting = await _settingRepository.FirstOrDefaultAsync(
                s => s.TenantId == settingInfo.TenantId && s.UserId == settingInfo.UserId && s.Name == settingInfo.Name
                );

            if (setting != null)
            {
                setting.Value = settingInfo.Value;
            }
        }
    }
}