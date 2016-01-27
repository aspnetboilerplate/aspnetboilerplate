using System;
using System.Collections.Generic;
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

        public Task<List<UserNotification>> GetUserNotifications(long userId, int skipCount = 0, int maxResultCount = Int32.MaxValue)
        {
            throw new NotImplementedException();
            //var userNotificationInfos = _store.GetUserNotifications(userId)
        }

        public Task<List<UserNotification>> GetUserNotification(Guid userNotificationId)
        {
            throw new NotImplementedException();
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