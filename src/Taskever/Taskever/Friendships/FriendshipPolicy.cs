using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Friendships
{
    public class FriendshipPolicy : IFriendshipPolicy
    {
        public bool CanChangeFriendshipProperties(AbpUser user, Friendship friendShip)
        {
            //Can change only his own friendships.
            return user.Id == friendShip.User.Id;
        }

        public bool CanRemoveFriendship(AbpUser currentUser, Friendship friendship)
        {
            return friendship.User.Id == currentUser.Id;
        }
    }
}