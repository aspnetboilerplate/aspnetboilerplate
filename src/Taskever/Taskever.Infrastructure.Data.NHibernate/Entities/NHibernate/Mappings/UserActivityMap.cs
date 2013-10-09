using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class UserActivityMap : EntityMap<UserActivity, long>
    {
        public UserActivityMap()
            : base("TeUserActivities")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Activity).Column("ActivityId").LazyLoad();
        }
    }
}