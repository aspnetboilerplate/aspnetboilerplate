using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Policies
{
    public interface IFriendshipPolicy : IPolicy
    {
        bool CanChangeFriendshipProperties(User currentUser, Friendship friendShip);
        bool CanRemoveFriendship(User currentUser, Friendship friendship);
    }
}