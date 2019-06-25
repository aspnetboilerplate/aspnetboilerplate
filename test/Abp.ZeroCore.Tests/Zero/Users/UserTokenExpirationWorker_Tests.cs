using System;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserTokenExpirationWorker_Tests : AbpZeroTestBase
    {
        private readonly UserTokenExpirationWorker _userTokenExpirationWorker;
        private readonly IRepository<UserToken, long> _userTokenRepository;
        private readonly AbpUserManager<Role, User> _abpUserManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserTokenExpirationWorker_Tests()
        {
            _userTokenExpirationWorker = Resolve<MyUserTokenExpirationWorker>();
            _userTokenRepository = Resolve<IRepository<UserToken, long>>();
            _abpUserManager = Resolve<AbpUserManager<Role, User>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Remove_Expired_TokenValidityKeys()
        {
            //Arrange
            using (_unitOfWorkManager.Begin())
            {
                var user = await _abpUserManager.GetUserByIdAsync(AbpSession.GetUserId());

                await _abpUserManager.AddTokenValidityKeyAsync(user, Guid.NewGuid().ToString(), DateTime.UtcNow);
                await _abpUserManager.AddTokenValidityKeyAsync(user, Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(1));
                await _abpUserManager.AddTokenValidityKeyAsync(user, Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(1));
                _unitOfWorkManager.Current.SaveChanges();

                var allTokens = _userTokenRepository.GetAllList(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(3);
            }

            //Act
            _userTokenExpirationWorker.Start();

            //Assert
            using (_unitOfWorkManager.Begin())
            {
                var user = await _abpUserManager.GetUserByIdAsync(AbpSession.GetUserId());
                var allTokens = _userTokenRepository.GetAllList(t => t.UserId == user.Id);
                allTokens.Count.ShouldBe(2);
            }
        }
    }

    internal class MyUserTokenExpirationWorker : UserTokenExpirationWorker
    {
        public MyUserTokenExpirationWorker(
            AbpTimer timer, 
            IRepository<UserToken, long> userTokenRepository, 
            IBackgroundJobConfiguration backgroundJobConfiguration)
            : base(timer, userTokenRepository, backgroundJobConfiguration)
        {   
        }

        public override void Start()
        {
            DoWork();
        }
    }
}
