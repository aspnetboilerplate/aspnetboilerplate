using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChanges")]
    public class EntityChangeInfo : Entity<long>, IMayHaveTenant
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
        /// ChangeTime.
        /// </summary>
        public virtual DateTime ChangeTime { get; set; }

        /// <summary>
        /// ChangeType.
        /// </summary>
        public virtual EntityChangeType ChangeType { get; set; }

        public virtual long EntityChangeSetId { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        [MaxLength(MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        /// AssemblyQualifiedName of the entity type.
        /// </summary>
        [MaxLength(MaxEntityTypeAssemblyQualifiedNameLength)]
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

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
        public virtual ICollection<EntityPropertyChangeInfo> PropertyChanges { get; set; }
    }
}
