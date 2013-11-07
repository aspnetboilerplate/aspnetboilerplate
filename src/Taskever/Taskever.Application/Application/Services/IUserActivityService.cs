using Abp.Application.Services;
using Taskever.Application.Services.Dto.Activities;

namespace Taskever.Application.Services
{
    public interface IUserActivityService : IApplicationService
    {
        GetFallowedActivitiesOutput GetFallowedActivities(GetFallowedActivitiesInput input);
        
        GetUserActivitiesOutput GetUserActivities(GetUserActivitiesInput input);
    }
}