using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Json;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store a notification subscription.
    /// </summary>
    [Table("AbpNotificationSubscriptions")]
    public class NotificationSubscriptionInfo : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Tenant id of the subscribed user.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        /// Notification unique name.
        /// </summary>
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }

        /// <summary>
        /// Gets/sets entity type name, if this is an entity level notification.
        /// It's FullName of the entity type.
        /// </summary>
        [MaxLength(NotificationInfo.MaxEntityTypeNameLength)]
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        [MaxLength(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength)]
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        [MaxLength(NotificationInfo.MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionInfo"/> class.
        /// </summary>
        public NotificationSubscriptionInfo()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionInfo"/> class.
        /// </summary>
        public NotificationSubscriptionInfo(int? tenantId, long userId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            TenantId = tenantId;
            NotificationName = notificationName;
            UserId = userId;
            EntityTypeName = entityIdentifier == null ? null : entityIdentifier.Type.FullName;
            EntityTypeAssemblyQualifiedName = entityIdentifier == null ? null : entityIdentifier.Type.AssemblyQualifiedName;
            EntityId = entityIdentifier == null ? null : entityIdentifier.Id.ToJsonString();
        }
    }
}