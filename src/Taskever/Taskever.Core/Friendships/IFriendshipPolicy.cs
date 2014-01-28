using Abp.Domain.Policies;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Friendships
{
    public interface IFriendshipPolicy : IPolicy
    {
        bool CanChangeFriendshipProperties(User currentUser, Friendship friendShip);
        bool CanRemoveFriendship(User currentUser, Friendship friendship);
    }
}