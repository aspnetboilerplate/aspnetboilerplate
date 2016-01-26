using System;
using Abp.Domain.Entities;
using Abp.Extensions;
using Newtonsoft.Json;

namespace Abp.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="NotificationInfo"/>.
    /// </summary>
    public static class NotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="NotificationInfo"/> to <see cref="Notification"/>.
        /// </summary>
        public static Notification ToNotification(this NotificationInfo notificationInfo)
        {
            var entityType = notificationInfo.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(notificationInfo.EntityTypeAssemblyQualifiedName);

            return new Notification
            {
                Id = notificationInfo.Id,
                NotificationName = notificationInfo.NotificationName,
                Data = JsonConvert.DeserializeObject(notificationInfo.Data, Type.GetType(notificationInfo.DataTypeName)),
                EntityTypeName = notificationInfo.EntityTypeName,
                EntityType = entityType,
                EntityId = notificationInfo.EntityId.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(notificationInfo.EntityId, EntityHelper.GetPrimaryKeyType(entityType)),
                Severity = notificationInfo.Severity,
                CreationTime = notificationInfo.CreationTime,
            };
        }
    }

    /// <summary>
    /// Extension methods for <see cref="UserNotificationInfo"/>.
    /// </summary>
    public static class UserNotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotificationInfo"/> to <see cref="UserNotification"/>.
        /// </summary>
        public static UserNotification ToUserNotification(this UserNotificationInfo userNotificationInfo, Notification notification)
        {
            return new UserNotification
            {
                Id = userNotificationInfo.Id,
                Notification = notification,
                UserId = userNotificationInfo.UserId,
                State = userNotificationInfo.State
            };
        }
    }
}
