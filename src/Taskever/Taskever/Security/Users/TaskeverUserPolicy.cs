using Abp.Security.Users;
using Taskever.Friendships;

namespace Taskever.Security.Users
{
    public class TaskeverUserPolicy : ITaskeverUserPolicy
    {
        private readonly IFriendshipDomainService _friendshipDomainService;

        public TaskeverUserPolicy(IFriendshipDomainService friendshipDomainService)
        {
            _friendshipDomainService = friendshipDomainService;
        }

        public bool CanSeeProfile(AbpUser requesterUser, AbpUser targetUser)
        {
            return requesterUser.Id == targetUser.Id || _friendshipDomainService.HasFriendship(requesterUser, targetUser);
        }
    }
}