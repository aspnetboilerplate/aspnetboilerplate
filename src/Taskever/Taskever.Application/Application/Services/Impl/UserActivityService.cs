using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Repositories;
using Taskever.Activities;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Friendships;

namespace Taskever.Application.Services.Impl
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFollowedActivityRepository _followedActivityRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IFriendshipDomainService _friendshipDomainService;

        public UserActivityService(IUserRepository userRepository, IUserFollowedActivityRepository followedActivityRepository, IActivityRepository activityRepository, IFriendshipDomainService friendshipDomainService)
        {
            _userRepository = userRepository;
            _followedActivityRepository = followedActivityRepository;
            _activityRepository = activityRepository;
            _friendshipDomainService = friendshipDomainService;
        }

        [UnitOfWork]
        public GetFollowedActivitiesOutput GetFollowedActivities(GetFollowedActivitiesInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var user = _userRepository.Load(input.UserId);

            //Can see activities of this user?
            if (currentUser.Id != user.Id && !_friendshipDomainService.HasFriendship(user, currentUser))
            {
                throw new AbpUserFriendlyException("Can not see activities of this user!");
            }

            //TODO: Think on private activities? When a private task is created or completed?

            var activities = _followedActivityRepository.Getactivities(input.UserId,input.IsActor, input.BeforeId, input.MaxResultCount);

            return new GetFollowedActivitiesOutput
                       {
                           Activities = activities.MapIList<UserFollowedActivity, UserFollowedActivityDto>()
                       };
        }
    }
}