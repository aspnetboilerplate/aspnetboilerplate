using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class ActivityMap : EntityMap<Activity, long>
    {
        public ActivityMap()
            : base("TeActivities")
        {
            References(x => x.ActorUser).Column("ActorUserId").LazyLoad();
            Map(x => x.Action).CustomType<ActivityAction>().Not.Nullable();
            Map(x => x.Data).Not.Nullable();
            Map(x => x.CreationTime);
        }
    }
}