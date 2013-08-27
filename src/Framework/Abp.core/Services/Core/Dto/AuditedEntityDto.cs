using System;

namespace Abp.Services.Core.Dto
{
    public abstract class AuditedEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user's id for this entity.
        /// </summary>
        public virtual int? CreatorId { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual int? LastModifierId { get; set; }
    }

    public abstract class AuditedEntityDto : AuditedEntityDto<int>
    {

    }
}