using System;
using Adorable.Domain.Entities;
using Adorable.Extensions;
using Newtonsoft.Json;

namespace Adorable.Notifications
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
                Data = notificationInfo.Data.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(notificationInfo.Data, Type.GetType(notificationInfo.DataTypeName)) as NotificationData,
                EntityTypeName = notificationInfo.EntityTypeName,
                EntityType = entityType,
                EntityId = notificationInfo.EntityId.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(notificationInfo.EntityId, EntityHelper.GetPrimaryKeyType(entityType)),
                Severity = notificationInfo.Severity,
                CreationTime = notificationInfo.CreationTime,
            };
        }
    }
}
