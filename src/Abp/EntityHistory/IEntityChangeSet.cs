using System.Collections.Generic;

namespace Abp.EntityHistory
{
    public interface IEntityChangeSet
    {
        IList<EntityChangeInfo> EntityChanges { get; set; }
    }
}
