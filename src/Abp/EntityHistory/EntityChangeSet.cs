using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChangeSets")]
    public class EntityChangeSet : Entity<long>, IEntityChangeSet, IMayHaveTenant
    {
        /// <summary>
        /// TenantId.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Entity changes grouped in this change set.
        /// </summary>
        public IList<EntityChangeInfo> EntityChanges { get; set; }

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChangeInfo>();
        }
    }
}
