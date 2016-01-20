using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    public class NullNotificationStore : INotificationStore
    {
        /// <summary>
        /// Gets single instance of <see cref="NullNotificationStore"/> class.
        /// </summary>
        public static NullNotificationStore Instance { get { return SingletonInstance; } }
        private static readonly NullNotificationStore SingletonInstance = new NullNotificationStore();

        private NullNotificationStore()
        {

        }

        public Task InsertSubscriptionAsync(NotificationSubscriptionOptions options)
        {
            return Task.FromResult(0);
        }

        public Task InsertNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        public Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            return Task.FromResult(0);
        }

        public Task<long[]> GetSubscribedUserIds(string name)
        {
            return Task.FromResult(new long[0]);
        }
    }
}