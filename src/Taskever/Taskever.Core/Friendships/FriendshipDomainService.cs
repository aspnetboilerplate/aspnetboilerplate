using System.Linq;
using Abp.Users;

namespace Taskever.Friendships
{
    public class FriendshipDomainService : IFriendshipDomainService
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipDomainService(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public bool HasFriendship(User user, User probableFriend)
        {
            return _friendshipRepository.Query( //TODO: Create Index: UserId, FriendId, Status
               q => q.Any(friendship =>
                          friendship.User.Id == user.Id &&
                          friendship.Friend.Id == probableFriend.Id &&
                          friendship.Status == FriendshipStatus.Accepted
                        ));
        }
    }
}