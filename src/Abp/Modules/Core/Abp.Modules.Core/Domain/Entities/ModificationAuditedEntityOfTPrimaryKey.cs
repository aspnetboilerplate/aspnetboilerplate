using System;
using Abp.Security.Users;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// This class can be used to simplify implementing IAudited.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public abstract class ModificationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IModificationAudited
    {
        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual AbpUser LastModifierUser { get; set; }
    }
}