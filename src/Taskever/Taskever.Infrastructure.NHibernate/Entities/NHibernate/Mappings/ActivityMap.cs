using Abp.Domain.Entities.Mapping;
using Taskever.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class ActivityMap : EntityMap<Activity, long>
    {
        public ActivityMap()
            : base("TeActivities")
        {
            DiscriminateSubClassesOnColumn("ActivityType");
            this.MapCreationTime();
        }
    }
}