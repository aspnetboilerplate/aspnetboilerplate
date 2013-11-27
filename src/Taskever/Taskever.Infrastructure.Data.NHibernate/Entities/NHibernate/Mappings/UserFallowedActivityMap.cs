using Abp.Modules.Core.Entities.NHibernate.Mappings;
using FluentNHibernate.Mapping;
using Taskever.Domain.Entities;

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
            Map(x => x.CreationTime);
        }
    }
}