using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities.EventHistories;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class EventHistoryMap : EntityMap<EventHistory, long>
    {
        public EventHistoryMap()
            : base("TeEventHistories")
        {
            Map(x => x.HistoryType).CustomType<EventHistoryType>().Not.Nullable();
            Map(x => x.HistoryVersion).Not.Nullable();
            Map(x => x.HistoryText).Not.Nullable();
            //Not.Map(x => x.Formatter);
        }
    }
}