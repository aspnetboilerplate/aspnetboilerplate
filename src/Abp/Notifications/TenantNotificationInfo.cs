using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Notifications
{
    /// <summary>
    /// A notification distributed to it's related tenant.
    /// </summary>
    [Table("AbpTenantNotifications")]
    public class TenantNotificationInfo : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Tenant id of the subscribed user.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Unique notification name.
        /// </summary>
        [Required]
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }

        /// <summary>
        /// Notification data as JSON string.
        /// </summary>
        [MaxLength(NotificationInfo.MaxDataLength)]
        public virtual string Data { get; set; }

        /// <summary>
        /// Type of the JSON serialized <see cref="Data"/>.
        /// It's AssemblyQualifiedName of the type.
        /// </summary>
        [MaxLength(NotificationInfo.MaxDataTypeNameLength)]
        public virtual string DataTypeName { get; set; }

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
        /// Notification severity.
        /// </summary>
        public virtual NotificationSeverity Severity { get; set; }

        public TenantNotificationInfo()
        {
            
        }

        public TenantNotificationInfo(int? tenantId, NotificationInfo notification)
        {
            TenantId = tenantId;
            NotificationName = notification.NotificationName;
            Data = notification.Data;
            DataTypeName = notification.DataTypeName;
            EntityTypeName = notification.EntityTypeName;
            EntityTypeAssemblyQualifiedName = notification.EntityTypeAssemblyQualifiedName;
            EntityId = notification.EntityId;
            Severity = notification.Severity;
        }
    }
}