using System;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Repositories;
using Taskever.Application.Services.Dto.TaskeverUsers;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class TaskeverUserService : ITaskeverUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskeverUserPolicy _taskeverUserPolicy;


        public TaskeverUserService(IUserRepository userRepository, ITaskeverUserPolicy taskeverUserPolicy)
        {
            _userRepository = userRepository;
            _taskeverUserPolicy = taskeverUserPolicy;
        }

        public GetUserProfileOutput GetUserProfile(GetUserProfileInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var profileUser = _userRepository.Load(input.UserId);

            if (!_taskeverUserPolicy.CanSeeProfile(currentUser, profileUser))
            {
                return new GetUserProfileOutput { CanNotSeeTheProfile = true };
            }

            var output = new GetUserProfileOutput();

            output.User = _userRepository.Get(input.UserId).MapTo<UserDto>();
            
            //TODO: Get tasks

            return output;
        }
    }
}