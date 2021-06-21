using System;
using Abp.Domain.Entities;
using Abp.Extensions;
using Newtonsoft.Json;

namespace Abp.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="NotificationInfo"/>.
    /// </summary>
    public static class TenantNotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="NotificationInfo"/> to <see cref="TenantNotification"/>.
        /// </summary>
        public static TenantNotification ToTenantNotification(this TenantNotificationInfo tenantNotificationInfo)
        {
            var entityType = tenantNotificationInfo.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(tenantNotificationInfo.EntityTypeAssemblyQualifiedName);

            return new TenantNotification
            {
                Id = tenantNotificationInfo.Id,
                TenantId = tenantNotificationInfo.TenantId,
                NotificationName = tenantNotificationInfo.NotificationName,
                Data = tenantNotificationInfo.Data.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(tenantNotificationInfo.Data, Type.GetType(tenantNotificationInfo.DataTypeName)) as NotificationData,
                EntityTypeName = tenantNotificationInfo.EntityTypeName,
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the EntityType property is removed
				EntityType = entityType,
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the EntityType property is removed
                EntityId = tenantNotificationInfo.EntityId.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject(tenantNotificationInfo.EntityId, EntityHelper.GetPrimaryKeyType(entityType)),
                Severity = tenantNotificationInfo.Severity,
                CreationTime = tenantNotificationInfo.CreationTime
            };
        }
    }
}
