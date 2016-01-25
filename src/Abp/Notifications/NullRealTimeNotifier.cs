using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    public class NullRealTimeNotifier : IRealTimeNotifier
    {
        /// <summary>
        /// Gets single instance of <see cref="NullRealTimeNotifier"/> class.
        /// </summary>
        public static NullRealTimeNotifier Instance { get { return SingletonInstance; } }
        private static readonly NullRealTimeNotifier SingletonInstance = new NullRealTimeNotifier();

        public Task SendNotificationAsync(long[] userIds, NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        public Task SendNotificationAsync(NotificationInfo notification, List<UserNotificationInfo> userNotifications)
        {
            return Task.FromResult(0);
        }
    }
}