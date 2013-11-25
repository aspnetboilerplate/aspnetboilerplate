using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class UserFallowedActivityMap : EntityMap<UserFallowedActivity, long>
    {
        public UserFallowedActivityMap()
            : base("TeUserFallowedActivities")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Activity).Column("ActivityId").LazyLoad();
            Map(x => x.IsActor);
            Map(x => x.CreationTime);
        }
    }
}