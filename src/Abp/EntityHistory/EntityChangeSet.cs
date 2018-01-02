using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChangeSets")]
    public class EntityChangeSet : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Entity changes grouped in this change set.
        /// </summary>
        public virtual IList<EntityChange> EntityChanges { get; set; }

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChange>();
        }
    }
}
