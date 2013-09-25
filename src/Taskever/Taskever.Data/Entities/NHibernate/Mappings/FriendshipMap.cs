using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class FriendshipMap : EntityMap<Friendship>
    {
        public FriendshipMap()
            : base("TeFriendships")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            References(x => x.Friend).Column("FriendUserId").LazyLoad();
            Map(x => x.Status).CustomType<FriendshipStatus>().Not.Nullable();
        }
    }
}