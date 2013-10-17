using System.Linq;
using Abp.Domain.Repositories;
using Abp.Modules.Core.Domain.Entities;
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

        public bool CanSeeTasksOfUser(User requesterUser, User userOfTasks)
        {
            if (requesterUser.Id == userOfTasks.Id)
            {
                return true;
            }

            return _friendshipRepository.Query( //TODO: Create Index: UserId, FriendId, Status
                q => q.Any(friendship =>
                           friendship.User.Id == requesterUser.Id &&
                           friendship.Friend.Id == userOfTasks.Id &&
                           friendship.Status == FriendshipStatus.Accepted
                         ));
        }
    }
}