using System.Linq;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Data.Repositories;

namespace Taskever.Application.Services.Impl
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserFallowedActivityRepository _fallowedActivityRepository;
        private readonly IActivityRepository _activityRepository;

        public UserActivityService(IUserFallowedActivityRepository fallowedActivityRepository, IActivityRepository activityRepository)
        {
            _fallowedActivityRepository = fallowedActivityRepository;
            _activityRepository = activityRepository;
        }

        public GetFallowedActivitiesOutput GetFallowedActivities(GetFallowedActivitiesInput input)
        {
            var activities = _fallowedActivityRepository.GetActivities(input.FallowerUserId);
            return new GetFallowedActivitiesOutput
                       {
                           Activities = activities.Select(ActivityDto.CreateFromActivity).ToList()
                       };
        }

        public GetUserActivitiesOutput GetUserActivities(GetUserActivitiesInput input)
        {
            //TODO: Check it current user can see this user's activities?

            var activities = _activityRepository.Query(q => q.Where(activity => activity.ActorUser.Id == input.UserId).ToList());
            return new GetUserActivitiesOutput
            {
                Activities = activities.Select(ActivityDto.CreateFromActivity).ToList()
            };
        }
    }
}