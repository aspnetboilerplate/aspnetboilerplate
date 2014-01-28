using Abp.Security.Users;
using Abp.Users;
using Taskever.Friendships;

namespace Taskever.Tasks
{
    public class TaskPolicy : ITaskPolicy
    {
        private readonly IFriendshipDomainService _friendshipDomainService;
        private readonly IFriendshipRepository _friendshipRepository;

        public TaskPolicy(IFriendshipDomainService friendshipDomainService, IFriendshipRepository friendshipRepository)
        {
            _friendshipDomainService = friendshipDomainService;
            _friendshipRepository = friendshipRepository;
        }

        public bool CanSeeTasksOfUser(User requesterUser, User userOfTasks)
        {
            return requesterUser.Id == userOfTasks.Id ||
                   _friendshipDomainService.HasFriendship(requesterUser, userOfTasks);
        }

        public bool CanAssignTask(User assignerUser, User userToAssign)
        {
            if (assignerUser.Id == userToAssign.Id) //TODO: Override == to be able to write just assignerUser == userToAssign
            {
                return true;
            }

            var friendship = _friendshipRepository.GetOrNull(assignerUser.Id, userToAssign.Id, true);
            if (friendship == null)
            {
                return false;
            }

            return friendship.CanAssignTask;
        }

        public bool CanUpdateTask(User user, Task task)
        {
            return (task.CreatorUser.Id == user.Id) || (task.AssignedUser.Id == user.Id);
        }

        public bool CanDeleteTask(User user, Task task)
        {
            return (task.CreatorUser.Id == user.Id) || (task.AssignedUser.Id == user.Id);
        }
    }
}