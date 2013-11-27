using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Services;

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

            var query = from fallowedActivity in _followedActivityRepository.GetAllWithActivity()
                        where fallowedActivity.User.Id == input.UserId && fallowedActivity.Id < input.BeforeId
                        select fallowedActivity;

            if (input.IsActor.HasValue)
            {
                query = query.Where(fa => fa.IsActor == input.IsActor.Value);
            }

            var activities = query
                .OrderByDescending(fa => fa.Id)
                .Take(input.MaxResultCount)
                .ToList();

            return new GetFollowedActivitiesOutput
                       {
                           Activities = activities.MapIList<UserFollowedActivity, UserFollowedActivityDto>()
                       };
        }
    }
}