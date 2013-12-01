using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class ActivityMap : EntityMap<Activity, long>
    {
        public ActivityMap()
            : base("TeActivities")
        {
            DiscriminateSubClassesOnColumn("ActivityType");
            Map(x => x.CreationTime);
        }
    }
}