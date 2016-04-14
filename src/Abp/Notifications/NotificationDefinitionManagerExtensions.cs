using Abp.Threading;
using System;
using System.Collections.Generic;

namespace Abp.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="INotificationDefinitionManager"/>.
    /// </summary>
    public static class NotificationDefinitionManagerExtensions
    {
        /// <summary>
        /// Gets all available notification definitions for given <see cref="tenantId"/> and <see cref="userId"/>.
        /// </summary>
        /// <param name="notificationDefinitionManager">Notification definition manager</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        public static IReadOnlyList<NotificationDefinition> GetAllAvailable(this INotificationDefinitionManager notificationDefinitionManager, Guid? tenantId, Guid userId)
        {
            return AsyncHelper.RunSync(() => notificationDefinitionManager.GetAllAvailableAsync(tenantId, userId));
        }
    }
}