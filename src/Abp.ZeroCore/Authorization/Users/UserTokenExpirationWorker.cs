using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;

namespace Abp.Authorization.Users
{
    public class UserTokenExpirationWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int IntervalInMilliseconds = 1 * 60 * 60 * 1000; // 1 hour

        private readonly IRepository<UserToken, long> _userTokenRepository;

        public UserTokenExpirationWorker(
            AbpTimer timer,
            IRepository<UserToken, long> userTokenRepository)
            : base(timer)
        {
            _userTokenRepository = userTokenRepository;

            Timer.Period = IntervalInMilliseconds;
        }

        protected override void DoWork()
        {
            var utcNow = Clock.Now.ToUniversalTime();
            _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
        }
    }
}
