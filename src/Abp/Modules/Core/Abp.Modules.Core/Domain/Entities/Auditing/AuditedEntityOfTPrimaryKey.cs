using System;
using Abp.Security.Users;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// This class can be used to simplify implementing IAudited.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public abstract class AuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual AbpUser CreatorUser { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual AbpUser LastModifierUser { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AuditedEntity()
        {
            CreationTime = DateTime.Now; //TODO: Set this property in the interceptor or somewhere else since it may break ORM system!
        }
    }
}