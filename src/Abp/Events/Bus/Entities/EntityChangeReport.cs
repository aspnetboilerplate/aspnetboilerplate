using System.Collections.Generic;
using System.Linq;

namespace Abp.Events.Bus.Entities
{
    public class EntityChangeReport
    {
        public List<EntityChangeEntry> ChangedEntities { get; }

        public List<DomainEventEntry> DomainEvents { get; }

        public EntityChangeReport()
        {
            ChangedEntities = new List<EntityChangeEntry>();
            DomainEvents = new List<DomainEventEntry>();
        }

        public bool IsEmpty()
        {
            return ChangedEntities.Count <= 0 && DomainEvents.Count <= 0;
        }

        public override string ToString()
        {
            return $"[EntityChangeReport] ChangedEntities: {ChangedEntities.Count}, DomainEvents: {DomainEvents.Count}";
        }
    }
}