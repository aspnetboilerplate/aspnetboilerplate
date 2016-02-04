using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements  <see cref="IUserNotificationManager"/>.
    /// </summary>
    public class UserNotificationManager : IUserNotificationManager, ISingletonDependency
    {
        private readonly INotificationStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
        /// </summary>
        public UserNotificationManager(INotificationStore store)
        {
            _store = store;
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(long userId, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            var userNotifications = await _store.GetUserNotificationsWithNotificationsAsync(userId, skipCount, maxResultCount);
            return userNotifications
                .Select(un => un.ToUserNotification())
                .ToList();
        }

        public async Task<UserNotification> GetUserNotificationAsync(Guid userNotificationId)
        {
            var userNotification = await _store.GetUserNotificationWithNotificationOrNullAsync(userNotificationId);
            if (userNotification == null)
            {
                return null;
            }

            return userNotification.ToUserNotification();
        }

        public Task UpdateUserNotificationStateAsync(Guid userNotificationId, UserNotificationState state)
        {
            return _store.UpdateUserNotificationStateAsync(userNotificationId, state);
        }

        public Task UpdateAllUserNotificationStatesAsync(long userId, UserNotificationState state)
        {
            return _store.UpdateAllUserNotificationStatesAsync(userId, state);
        }

        public Task DeleteUserNotificationAsync(Guid userNotificationId)
        {
            return _store.DeleteUserNotificationAsync(userNotificationId);
        }

        public Task DeleteAllUserNotificationsAsync(long userId)
        {
            return _store.DeleteAllUserNotificationsAsync(userId);
        }
    }
}