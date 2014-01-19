using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace Abp.Modules.Core.Application.Services.Dto
{
    public abstract class AuditedEntityDto : AuditedEntityDto<int>
    {

    }

    /// <summary>
    /// This class can be inherited for simple Dto objects those are used for entities those implement <see cref="IAudited"/> interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
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