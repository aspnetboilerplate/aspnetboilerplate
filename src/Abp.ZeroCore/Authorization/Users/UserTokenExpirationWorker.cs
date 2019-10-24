using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;

namespace Abp.Authorization.Users
{
    public class UserTokenExpirationWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int IntervalInMilliseconds = 1 * 60 * 60 * 1000; // 1 hour

        private readonly IRepository<UserToken, long> _userTokenRepository;
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserTokenExpirationWorker(
            AbpTimer timer,
            IRepository<UserToken, long> userTokenRepository,
            IBackgroundJobConfiguration backgroundJobConfiguration, 
            IUnitOfWorkManager unitOfWorkManager)
            : base(timer)
        {
            _userTokenRepository = userTokenRepository;
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _unitOfWorkManager = unitOfWorkManager;

            Timer.Period = GetTimerPeriod();
        }

        private int GetTimerPeriod()
        {
            if (_backgroundJobConfiguration.CleanUserTokenPeriod.HasValue)
            {
                return _backgroundJobConfiguration.CleanUserTokenPeriod.Value;
            }

            return IntervalInMilliseconds;
        }

        protected override void DoWork()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var utcNow = Clock.Now.ToUniversalTime();
                    _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                    uow.Complete();
                }
            }
        }
    }
}
