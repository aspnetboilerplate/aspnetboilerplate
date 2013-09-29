using System.Linq;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Services.Impl
{
    public class TaskPrivilegeService : ITaskPrivilegeService
    {
        private readonly IRepository<Friendship> _friendshipRepository;

        public TaskPrivilegeService(IRepository<Friendship> friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public bool CanSeeTasksOfUser(int currentUserId, int userId)
        {
            if (currentUserId == userId)
            {
                return true;
            }

            return _friendshipRepository.Query( //TODO: Create Index: UserId, FriendId, Status
                q => q.Any(friendship =>
                           friendship.User.Id == currentUserId &&
                           friendship.Friend.Id == userId &&
                           friendship.Status == FriendshipStatus.Accepted
                         ));
        }
    }
}