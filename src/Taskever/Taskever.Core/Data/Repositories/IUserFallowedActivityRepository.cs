using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Data.Repositories
{
    public interface IUserFollowedActivityRepository :  IRepository<UserFollowedActivity, long>
    {
        IQueryable<UserFollowedActivity> GetAllWithActivity();
    }
}