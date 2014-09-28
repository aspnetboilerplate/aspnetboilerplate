using System;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// Implements <see cref="IFullAudited"/> to be a base class for full-audited entities.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public abstract class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAudited
    {
        /// <summary>
        /// Is this entity Deleted?
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Which user deleted this entity?
        /// </summary>
        public long? DeleterUserId { get; set; }
        
        /// <summary>
        /// Deletion time of this entity.
        /// </summary>
        public DateTime? DeletionTime { get; set; }
    }
}