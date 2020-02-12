using System.Threading.Tasks;
using Abp.Notifications;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Notifications
{
    public class UserNotificationManager_Tests : SampleAppTestBase
    {
        private readonly IUserNotificationManager _userNotificationManager;

        public UserNotificationManager_Tests()
        {
            _userNotificationManager = Resolve<IUserNotificationManager>();
        }

        [Fact]
        public async Task GetUserNotificationCountAsync_Test()
        {
            var notificationCount = await _userNotificationManager.GetUserNotificationCountAsync(
                new UserIdentifier(1, 2), UserNotificationState.Read
            );

            notificationCount.ShouldBeGreaterThanOrEqualTo(0);
        }
    }
}
