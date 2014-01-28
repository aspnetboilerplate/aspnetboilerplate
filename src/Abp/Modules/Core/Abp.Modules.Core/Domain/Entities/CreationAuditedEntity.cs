using System;
using Abp.Security.Users;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="CreationAuditedEntity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class CreationAuditedEntity : CreationAuditedEntity<int>
    {

    }

    /// <summary>
    /// This class can be used to simplify implementing ICreationAudited.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited
    {
        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual User CreatorUser { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now; //TODO: Set this property in the interceptor or somewhere else since it may break ORM system!
        }
    }
}