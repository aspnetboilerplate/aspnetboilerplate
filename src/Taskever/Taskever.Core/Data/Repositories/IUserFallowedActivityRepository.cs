using System.Collections.Generic;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Data.Repositories
{
    public interface IUserFollowedActivityRepository :  IRepository<UserFollowedActivity, long>
    {
        IList<Activity> GetActivities(int fallowerUserId, int maxResultCount, long beforeFallowedActivityId);
    }
}