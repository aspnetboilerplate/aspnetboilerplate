using System;
using Abp.Domain.Entities.Auditing;

namespace Abp.Application.Services.Dto
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
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user's id for this entity.
        /// </summary>
        public int? CreatorUserId { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public int? LastModifierUserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AuditedEntityDto()
        {
            CreationTime = DateTime.Now;
        }
    }
}