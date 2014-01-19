using System;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Users;
using Taskever.Application.Services.Dto.TaskeverUsers;
using Taskever.Friendships;
using Taskever.Users;

namespace Taskever.Application.Services.Impl
{
    public class TaskeverUserService : ITaskeverUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskeverUserPolicy _taskeverUserPolicy;
        private readonly IFriendshipRepository _friendshipRepository;

        public TaskeverUserService(IUserRepository userRepository, ITaskeverUserPolicy taskeverUserPolicy, IFriendshipRepository friendshipRepository)
        {
            _userRepository = userRepository;
            _taskeverUserPolicy = taskeverUserPolicy;
            _friendshipRepository = friendshipRepository;
        }

        public GetUserProfileOutput GetUserProfile(GetUserProfileInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);

            var profileUser = _userRepository.Get(input.UserId);
            if (profileUser == null)
            {
                throw new AbpUserFriendlyException("Can not found the user!");
            }

            if (profileUser.Id != currentUser.Id)
            {
                var friendship = _friendshipRepository.GetOrNull(currentUser.Id, input.UserId);
                if (friendship == null)
                {
                    return new GetUserProfileOutput { CanNotSeeTheProfile = true, User = profileUser.MapTo<UserDto>() }; //TODO: Is it true to send user informations?
                }

                if (friendship.Status != FriendshipStatus.Accepted)
                {
                    return new GetUserProfileOutput { CanNotSeeTheProfile = true, SentFriendshipRequest = true, User = profileUser.MapTo<UserDto>() };
                }
            }

            return new GetUserProfileOutput { User = profileUser.MapTo<UserDto>() };
        }
    }
}