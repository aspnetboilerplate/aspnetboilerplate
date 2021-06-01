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
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingStore(
            IRepository<Setting, long> settingRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _settingRepository = settingRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            /* Combined SetTenantId and DisableFilter for backward compatibility.
             * SetTenantId switches database (for tenant) if needed.
             * DisableFilter and Where condition ensures to work even if tenantId is null for single db approach.
             */

            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var settingList = await _settingRepository.GetAllListAsync(s =>
                            s.UserId == userId && s.TenantId == tenantId
                        );

                        return settingList
                            .Select(s => s.ToSettingInfo())
                            .ToList();
                    }
                }
            });
        }

        public virtual List<SettingInfo> GetAllList(int? tenantId, long? userId)
        {
            /* Combined SetTenantId and DisableFilter for backward compatibility.
             * SetTenantId switches database (for tenant) if needed.
             * DisableFilter and Where condition ensures to work even if tenantId is null for single db approach.
             */

            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        return _settingRepository.GetAllList(s =>
                                s.UserId == userId && s.TenantId == tenantId
                            )
                            .Select(s => s.ToSettingInfo())
                            .ToList();
                    }
                }

            });
        }

        public virtual async Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var settingInfo = await _settingRepository.FirstOrDefaultAsync(s =>
                            s.UserId == userId && s.Name == name && s.TenantId == tenantId
                        );

                        return settingInfo.ToSettingInfo();
                    }
                }
            });
        }

        public virtual SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        return _settingRepository.FirstOrDefault(s =>
                            s.UserId == userId && s.Name == name && s.TenantId == tenantId
                        ).ToSettingInfo();
                    }
                }
            });
        }

        public virtual async Task DeleteAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        await _settingRepository.DeleteAsync(
                            s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name &&
                                 s.TenantId == settingInfo.TenantId
                        );
                        
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Delete(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        _settingRepository.Delete(
                            s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name &&
                                 s.TenantId == settingInfo.TenantId
                        );
                    
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }

        public virtual async Task CreateAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        await _settingRepository.InsertAsync(settingInfo.ToSetting());
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Create(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        _settingRepository.Insert(settingInfo.ToSetting());
                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }

        public virtual async Task UpdateAsync(SettingInfo settingInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var setting = await _settingRepository.FirstOrDefaultAsync(
                            s => s.TenantId == settingInfo.TenantId &&
                                 s.UserId == settingInfo.UserId &&
                                 s.Name == settingInfo.Name
                        );

                        if (setting != null)
                        {
                            setting.Value = settingInfo.Value;
                        }

                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }
            });
        }

        public virtual void Update(SettingInfo settingInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        var setting = _settingRepository.FirstOrDefault(
                            s => s.TenantId == settingInfo.TenantId &&
                                 s.UserId == settingInfo.UserId &&
                                 s.Name == settingInfo.Name
                        );

                        if (setting != null)
                        {
                            setting.Value = settingInfo.Value;
                        }

                        _unitOfWorkManager.Current.SaveChanges();
                    }
                }
            });
        }
    }
}
