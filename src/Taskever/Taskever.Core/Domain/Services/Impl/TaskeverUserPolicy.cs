using System;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Services.Impl
{
    public class TaskeverUserPolicy : ITaskeverUserPolicy
    {
        private readonly IFriendshipDomainService _friendshipDomainService;

        public TaskeverUserPolicy(IFriendshipDomainService friendshipDomainService)
        {
            _friendshipDomainService = friendshipDomainService;
        }

        public bool CanSeeProfile(User requesterUser, User targetUser)
        {
            return requesterUser.Id == targetUser.Id || _friendshipDomainService.HasFriendship(requesterUser, targetUser);
        }
    }
}