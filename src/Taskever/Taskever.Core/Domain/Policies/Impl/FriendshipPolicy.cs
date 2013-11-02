using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Policies.Impl
{
    public class FriendshipPolicy : IFriendshipPolicy
    {
        public bool CanChangeFriendshipProperties(User user, Friendship friendShip)
        {
            //Can change only his own friendships.
            return user.Id == friendShip.User.Id;
        }

        public bool CanRemoveFriendship(User currentUser, Friendship friendship)
        {
            return friendship.User.Id == currentUser.Id;
        }
    }
}