using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChangeSets")]
    public class EntityChangeSet : Entity<long>, IEntityChangeSet, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public IList<EntityChangeInfo> EntityChanges { get; set; }

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChangeInfo>();
        }
    }
}
