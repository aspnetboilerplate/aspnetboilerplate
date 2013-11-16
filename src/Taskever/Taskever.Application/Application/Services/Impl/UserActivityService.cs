using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Data.Repositories;
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
            var activities = _fallowedActivityRepository.GetActivities(input.FallowerUserId);
            return new GetFallowedActivitiesOutput
                       {
                           Activities = activities.Select(ActivityDto.CreateFromActivity).ToList()
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
            
            var activities = _activityRepository.Query(
                q => q.Where(activity => activity.ActorUser.Id == input.UserId && activity.Id < input.BeforeActivityId)
                    .OrderByDescending(activity => activity.Id)
                    .Take(input.MaxResultCount)
                    .ToList());

            return new GetUserActivitiesOutput
            {
                Activities = activities.Select(ActivityDto.CreateFromActivity).ToList()
            };
        }
    }
}