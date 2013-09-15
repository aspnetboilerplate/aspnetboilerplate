using System;
using Abp.Entities;

namespace Abp.Modules.Core.Entities.Utils
{
    /// <summary>
    /// A shortcut of <see cref="ModificationAuditedEntity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class ModificationAuditedEntity : ModificationAuditedEntity<int>
    {

    }

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
        public virtual User LastModifierUser { get; set; }
    }
}