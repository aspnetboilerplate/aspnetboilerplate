using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Notifications;
using Abp.Timing;
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
        public void Should_Delete_Subscription_When_User_Deleted()
        {
            var notificationName = "Test";
            var user = CreateAndGetUser();

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

        private User CreateAndGetUser()
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "test@aspnetboilerplate.com",
                    Name = "Test Name",
                    Surname = "Test Surname",
                    UserName = "test.username"
                });

            return UsingDbContext(
                context => { return context.Users.First(u => u.UserName == "test.username"); });
        }
    }
}