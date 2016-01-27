using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Json;

namespace Abp.Notifications
{
    [Table("AbpNotificationSubscriptions")]
    public class NotificationSubscriptionInfo : CreationAuditedEntity<Guid>
    {
        public virtual long UserId { get; set; }

        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }
        
        [MaxLength(NotificationInfo.MaxEntityTypeNameLength)]
        public virtual string EntityTypeName { get; set; }

        [MaxLength(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength)]
        public string EntityTypeAssemblyQualifiedName { get; set; }

        [MaxLength(NotificationInfo.MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        public NotificationSubscriptionInfo()
        {
            
        }

        public NotificationSubscriptionInfo(long userId, string notificationName, Type entityType = null, object entityId = null)
        {
            NotificationName = notificationName;
            UserId = userId;
            EntityTypeName = entityType == null ? null : entityType.FullName;
            EntityTypeAssemblyQualifiedName = entityType == null ? null : entityType.AssemblyQualifiedName;
            EntityId = entityId == null ? null : entityId.ToJsonString();
        }
    }
}