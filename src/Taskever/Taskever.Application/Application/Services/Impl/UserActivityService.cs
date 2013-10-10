using System.Linq;
using Taskever.Application.Services.Dto.Activities;
using Taskever.Data.Repositories;
using Taskever.Domain.Business.Acitivities;

namespace Taskever.Application.Services.Impl
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserFallowedActivityRepository _fallowedActivityRepository;

        public UserActivityService(IUserFallowedActivityRepository fallowedActivityRepository)
        {
            _fallowedActivityRepository = fallowedActivityRepository;
        }
        
        public GetFallowedActivitiesOutput GetFallowedActivities(GetFallowedActivitiesInput input)
        {
            var activities = _fallowedActivityRepository.GetActivities(input.FallowerUserId);
            return new GetFallowedActivitiesOutput
                       {
                           Activities = activities.Select(
                               activity =>
                               new ActivityDto
                                   {
                                       Action = activity.Action,
                                       ActivityInfo = activity.Action.CreateActivityInfo(activity.Data),
                                       CreationTime = activity.CreationTime
                                   }
                               ).ToList()
                       };
        }
    }
}