using System.Collections.Generic;

namespace Abp.EntityHistory
{
    public class EntityChangeSet : IEntityChangeSet
    {
        public IList<EntityChangeInfo> EntityChanges { get; set; }

        public IList<EntityPropertyChangeInfo> EntityPropertyChanges { get; set; }

        EntityChangeSet()
        {
            EntityChanges = new List<EntityChangeInfo>();
            EntityPropertyChanges = new List<EntityPropertyChangeInfo>();
        }
    }
}
