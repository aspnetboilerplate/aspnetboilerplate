using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class ActivityMap : EntityMap<Activity, long>
    {
        public ActivityMap()
            : base("TeActivities")
        {
            DiscriminateSubClassesOnColumn("ActivityType");
            Map(x => x.CreationTime);

            //Cache.ReadOnly(); //TODO: Try caches!
        }
    }
}