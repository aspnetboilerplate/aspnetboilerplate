using System;
using Abp.Services.Dto;

namespace Abp.Modules.Core.Services.Dto
{
    public abstract class AuditedEntityDto : AuditedEntityDto<int>
    {

    }

    public abstract class AuditedEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user's id for this entity.
        /// </summary>
        public virtual int? CreatorUserId { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual int? LastModifierUserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AuditedEntityDto()
        {
            CreationTime = DateTime.Now;
        }
    }
}