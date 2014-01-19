using Abp.Application.Services;
using Taskever.Activities.Dto;

namespace Taskever.Activities
{
    public interface IUserActivityAppService : IApplicationService
    {
        GetFollowedActivitiesOutput GetFollowedActivities(GetFollowedActivitiesInput input);
    }
}