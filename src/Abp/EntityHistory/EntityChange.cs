using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChanges")]
    public class EntityChange : Entity<long>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of <see cref="EntityId"/> property.
        /// Value: 96.
        /// </summary>
        public const int MaxEntityIdLength = 96;

        /// <summary>
        /// Maximum length of <see cref="EntityTypeAssemblyQualifiedName"/> property.
        /// Value: 512.
        /// </summary>
        public const int MaxEntityTypeAssemblyQualifiedNameLength = 512;

        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public const int MaxBrowserInfoLength = 256;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public const int MaxClientNameLength = 128;

        /// <summary>
        /// ChangeTime.
        /// </summary>
        public virtual DateTime ChangeTime { get; set; }

        /// <summary>
        /// ChangeType.
        /// </summary>
        public virtual EntityChangeType ChangeType { get; set; }

        /// <summary>
        /// Gets/sets change set id, used to group entity changes.
        /// </summary>
        public virtual long EntityChangeSetId { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity.
        /// </summary>
        [MaxLength(MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        [MaxLength(MaxEntityTypeAssemblyQualifiedNameLength)]
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Browser information if this entity is changed in a web request.
        /// </summary>
        [MaxLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        [MaxLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        [MaxLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        /// ImpersonatorTenantId.
        /// </summary>
        public virtual int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// ImpersonatorUserId.
        /// </summary>
        public virtual long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        /// PropertyChanges.
        /// </summary>
        public virtual ICollection<EntityPropertyChange> PropertyChanges { get; set; }

        #region Not mapped

        [NotMapped]
        public virtual object EntityEntry { get; set; }

        #endregion
    }
}
