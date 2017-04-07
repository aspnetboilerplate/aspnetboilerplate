using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to manage user notifications.
    /// </summary>
    public interface IUserNotificationManager
    {
        /// <summary>
        /// Gets notifications for a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">State</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue);

        /// <summary>
        /// Gets user notification count.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">State.</param>
        Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null);

        /// <summary>
        /// Gets a user notification by given id.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">The user notification id.</param>
        Task<UserNotification> GetUserNotificationAsync(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        /// <param name="tenantId">Tenant Id.</param>
        /// <param name="userNotificationId">The user notification id.</param>
        /// <param name="state">New state.</param>
        Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state);

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">New state.</param>
        Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state);

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        /// <param name="tenantId">Tenant Id.</param>
        /// <param name="userNotificationId">The user notification id.</param>
        Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        /// <param name="user">User.</param>
        Task DeleteAllUserNotificationsAsync(UserIdentifier user);
    }
}