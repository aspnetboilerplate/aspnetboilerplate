using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store a notification request.
    /// This notification is distributed to tenants and users by <see cref="INotificationDistributer"/>.
    /// </summary>
    [Serializable]
    [Table("AbpNotifications")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class NotificationInfo : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// Indicates all tenant ids for <see cref="TenantIds"/> property.
        /// Value: "0".
        /// </summary>
        public const string AllTenantIds = "0";

        /// <summary>
        /// Maximum length of <see cref="NotificationName"/> property.
        /// Value: 96.
        /// </summary>
        public const int MaxNotificationNameLength = 96;

        /// <summary>
        /// Maximum length of <see cref="Data"/> property.
        /// Value: 1048576 (1 MB).
        /// </summary>
        public const int MaxDataLength = 1024 * 1024;

        /// <summary>
        /// Maximum length of <see cref="DataTypeName"/> property.
        /// Value: 512.
        /// </summary>
        public const int MaxDataTypeNameLength = 512;

        /// <summary>
        /// Maximum length of <see cref="EntityTypeName"/> property.
        /// Value: 250.
        /// </summary>
        public const int MaxEntityTypeNameLength = 250;

        /// <summary>
        /// Maximum length of <see cref="EntityTypeAssemblyQualifiedName"/> property.
        /// Value: 512.
        /// </summary>
        public const int MaxEntityTypeAssemblyQualifiedNameLength = 512;

        /// <summary>
        /// Maximum length of <see cref="EntityId"/> property.
        /// Value: 96.
        /// </summary>
        public const int MaxEntityIdLength = 96;

        /// <summary>
        /// Maximum length of <see cref="UserIds"/> property.
        /// Value: 131072 (128 KB).
        /// </summary>
        public const int MaxUserIdsLength = 128 * 1024;

        /// <summary>
        /// Maximum length of <see cref="TenantIds"/> property.
        /// Value: 131072 (128 KB).
        /// </summary>
        public const int MaxTenantIdsLength = 128 * 1024;

        /// <summary>
        /// Unique notification name.
        /// </summary>
        [Required]
        [StringLength(MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }

        /// <summary>
        /// Notification data as JSON string.
        /// </summary>
        [StringLength(MaxDataLength)]
        public virtual string Data { get; set; }

        /// <summary>
        /// Type of the JSON serialized <see cref="Data"/>.
        /// It's AssemblyQualifiedName of the type.
        /// </summary>
        [StringLength(MaxDataTypeNameLength)]
        public virtual string DataTypeName { get; set; }

        /// <summary>
        /// Gets/sets entity type name, if this is an entity level notification.
        /// It's FullName of the entity type.
        /// </summary>
        [StringLength(MaxEntityTypeNameLength)]
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        [StringLength(MaxEntityTypeAssemblyQualifiedNameLength)]
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        [StringLength(MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        /// Notification severity.
        /// </summary>
        public virtual NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Target users of the notification.
        /// If this is set, it overrides subscribed users.
        /// If this is null/empty, then notification is sent to all subscribed users.
        /// </summary>
        [StringLength(MaxUserIdsLength)]
        public virtual string UserIds { get; set; }

        /// <summary>
        /// Excluded users.
        /// This can be set to exclude some users while publishing notifications to subscribed users.
        /// It's not normally used if <see cref="UserIds"/> is not null.
        /// </summary>
        [StringLength(MaxUserIdsLength)]
        public virtual string ExcludedUserIds { get; set; }

        /// <summary>
        /// Target tenants of the notification.
        /// Used to send notification to subscribed users of specific tenant(s).
        /// This is valid only if UserIds is null.
        /// If it's "0", then indicates to all tenants.
        /// </summary>
        [StringLength(MaxTenantIdsLength)]
        public virtual string TenantIds { get; set; }

        public NotificationInfo()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        public NotificationInfo(Guid id)
        {
            Id = id;
            Severity = NotificationSeverity.Info;
        }
    }
}