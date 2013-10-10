using System.Collections.Generic;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories
{
    public interface IUserFallowedActivityRepository :  IRepository<UserFallowedActivity, long>
    {
        IList<Activity> GetActivities(int fallowerUserId);
    }
}