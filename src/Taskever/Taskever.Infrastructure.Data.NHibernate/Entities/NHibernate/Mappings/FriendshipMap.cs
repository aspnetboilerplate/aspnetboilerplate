using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class FriendshipMap : EntityMap<Friendship>
    {
        public FriendshipMap()
            : base("TeFriendships")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Friend).Column("FriendUserId").LazyLoad();
            Map(x => x.FallowActivities).Column("FallowActivities");
            Map(x => x.CanAssignTask).Column("CanAssignTask");
            Map(x => x.Status).CustomType<FriendshipStatus>().Not.Nullable();
        }
    }
}