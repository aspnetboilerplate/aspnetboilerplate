using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Services.Impl
{
    public class TaskPrivilegeService : ITaskPrivilegeService
    {
        private readonly IFriendshipDomainService _friendshipDomainService;

        public TaskPrivilegeService(IFriendshipDomainService friendshipDomainService)
        {
            _friendshipDomainService = friendshipDomainService;
        }

        public bool CanSeeTasksOfUser(User requesterUser, User userOfTasks)
        {
            return requesterUser.Id == userOfTasks.Id ||
                   _friendshipDomainService.HasFriendship(requesterUser, userOfTasks);
        }
    }
}