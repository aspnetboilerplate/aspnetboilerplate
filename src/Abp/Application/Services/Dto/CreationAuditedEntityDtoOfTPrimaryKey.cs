using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This class can be inherited for simple Dto objects those are used for entities implement <see cref="ICreationAudited"/> interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of primary key</typeparam>
    [Serializable]
    public abstract class CreationAuditedEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user's id for this entity.
        /// </summary>
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CreationAuditedEntityDto()
        {
            CreationTime = Clock.Now;
        }
    }
}