using System;
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

        public Task DeleteSubscriptionAsync(NotificationSubscriptionOptions options)
        {
            return Task.FromResult(0);
        }

        public Task InsertNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        public Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            return Task.FromResult(null as NotificationInfo);
        }

        public Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            return Task.FromResult(0);
        }

        public Task<long[]> GetSubscribedUserIdsAsync(NotificationInfo notification)
        {
            return Task.FromResult(new long[0]);
        }
    }
}