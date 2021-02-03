using System.Linq;
using Abp.Domain.Uow;
using Abp.Notifications;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Notifications
{
    public class NotificationSubscriptionSynchronizer_Tests : SampleAppTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;

        public NotificationSubscriptionSynchronizer_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _tenantManager = Resolve<TenantManager>();
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _notificationSubscriptionManager = Resolve<INotificationSubscriptionManager>();
        }

        [Fact]
        public void Should_Delete_Subscription_When_HostUser_Deleted()
        {
            TestNotificationSubscriptionDeletion(null);
        }

        [Fact]
        public void Should_Delete_Subscription_When_TenantUser_Deleted()
        {
            AbpSession.TenantId = 1;
            TestNotificationSubscriptionDeletion(AbpSession.TenantId);
        }

        private void TestNotificationSubscriptionDeletion(int? tenantId)
        {
            var notificationName = "Test";
            var user = CreateTestUser(tenantId);

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == user.UserName);
                    userAccount.IsDeleted.ShouldBe(false);
                });

            _notificationSubscriptionManager.Subscribe(new UserIdentifier(user.TenantId, user.Id), notificationName);

            var subscriptions = _notificationSubscriptionManager.GetSubscriptions(notificationName);
            subscriptions.Count.ShouldBe(1);
            subscriptions[0].UserId.ShouldBe(user.Id);

            _userAppService.DeleteUser(user.Id);

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == user.UserName);
                    userAccount.IsDeleted.ShouldBe(true);
                });

            subscriptions = _notificationSubscriptionManager.GetSubscriptions(notificationName);
            subscriptions.Count.ShouldBe(0);
        }

        private User CreateTestUser(int? tenantId)
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "test@aspnetboilerplate.com",
                    Name = "Test Name",
                    Surname = "Test Surname",
                    UserName = "test.username",
                    TenantId = tenantId
                });

            return UsingDbContext(context =>
            {
                return context.Users.First(u => u.UserName == "test.username");
            });
        }
    }
}
