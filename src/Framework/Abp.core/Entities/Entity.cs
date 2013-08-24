using System;
using Abp.Entities.Core;

namespace Abp.Entities
{
    /// <summary>
    /// Basic implementation of IEntity interface.
    /// An entity can inherit this class of directly implement to IEntity interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        public virtual TPrimaryKey Id { get; set; }
    }

    public abstract class AuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual User Creator { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public virtual DateTime? LastModificationDate { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public virtual User LastModifier { get; set; }

        protected AuditedEntity()
        {
            CreationDate = DateTime.Now;
            LastModificationDate = DateTime.Now;
        }
    }

}