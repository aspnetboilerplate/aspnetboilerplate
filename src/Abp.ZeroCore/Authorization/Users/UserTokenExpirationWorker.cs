using System;
using System.Collections.Generic;
using System.Linq;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;

namespace Abp.Authorization.Users
{
    public class UserTokenExpirationWorker<TTenant, TUser> : PeriodicBackgroundWorkerBase
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase
    {
        private readonly IRepository<UserToken, long> _userTokenRepository;
        private readonly IRepository<TTenant> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserTokenExpirationWorker(
            AbpTimer timer,
            IRepository<UserToken, long> userTokenRepository,
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TTenant> tenantRepository)
            : base(timer)
        {
            _userTokenRepository = userTokenRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;

            Timer.Period = backgroundJobConfiguration.UserTokenExpirationPeriod?.TotalMilliseconds.To<int>()
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once support for CleanUserTokenPeriod property is removed
                           ?? backgroundJobConfiguration.CleanUserTokenPeriod
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once support for CleanUserTokenPeriod property is removed
                           ?? TimeSpan.FromHours(1).TotalMilliseconds.To<int>();
        }

        protected override void DoWork()
        {
            List<int> tenantIds;
            var utcNow = Clock.Now.ToUniversalTime();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                    tenantIds = _tenantRepository.GetAll().Select(t => t.Id).ToList();
                    uow.Complete();
                }
            }

            foreach (var tenantId in tenantIds)
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                        uow.Complete();
                    }
                }
            }
        }
    }
}
