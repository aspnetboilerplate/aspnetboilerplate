using Abp.Domain.Policies;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Friendships
{
    public interface IFriendshipPolicy : IPolicy
    {
        bool CanChangeFriendshipProperties(User currentUser, Friendship friendShip);
        bool CanRemoveFriendship(User currentUser, Friendship friendship);
    }
}