using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFallowedActivityRepository _fallowedActivityRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IFriendshipDomainService _friendshipDomainService;

        public UserActivityService(IUserRepository userRepository, IUserFallowedActivityRepository fallowedActivityRepository, IActivityRepository activityRepository, IFriendshipDomainService friendshipDomainService)
        {
            _userRepository = userRepository;
            _fallowedActivityRepository = fallowedActivityRepository;
            _activityRepository = activityRepository;
            _friendshipDomainService = friendshipDomainService;
        }

        public GetFallowedActivitiesOutput GetFallowedActivities(GetFallowedActivitiesInput input)
        {
            var activities = _fallowedActivityRepository.GetActivities(input.FallowerUserId, input.MaxResultCount, input.BeforeFallowedActivityId);
            return new GetFallowedActivitiesOutput
                       {
                           Activities = null //activities.Select(ActivityDto.CreateFromActivity).ToList()
                       };
        }

        [UnitOfWork]
        public GetUserActivitiesOutput GetUserActivities(GetUserActivitiesInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var user = _userRepository.Load(input.UserId);

            if (currentUser.Id != user.Id && !_friendshipDomainService.HasFriendship(user, currentUser))
            {
                throw new AbpUserFriendlyException("Can not see activities of this user!");
            }

            var activities = _fallowedActivityRepository.Query(
                q => q.Where(fa => fa.IsActor && fa.User.Id == input.UserId)
                    .OrderByDescending(activity => activity.Id)
                    .Take(input.MaxResultCount)
                    .Select(fa => fa.Activity)
                    .ToList()
                );

            var activityDtos = activities.MapIList<Activity, ActivityDto>();

            return new GetUserActivitiesOutput
            {
                Activities = activityDtos
            };
        }
    }
}