using Abp.Domain.Entities.Mapping;
using Taskever.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class UserFollowedActivityMap : EntityMap<UserFollowedActivity, long>
    {
        public UserFollowedActivityMap()
            : base("TeUserFollowedActivities")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Activity).Column("ActivityId").LazyLoad();
            Map(x => x.IsActor);
            this.MapCreationTime();
        }
    }
}