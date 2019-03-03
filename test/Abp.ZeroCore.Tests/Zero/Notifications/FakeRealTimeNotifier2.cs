using System.Threading.Tasks;
using Abp.Notifications;

namespace Abp.Zero.Notifications
{
    public class FakeRealTimeNotifier2 : IRealTimeNotifier
    {
        public bool IsSendCalled { get; set; }

        public Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            IsSendCalled = true;
            return Task.CompletedTask;
        }
    }
}
