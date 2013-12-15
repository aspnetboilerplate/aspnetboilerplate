using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Repositories
{
    public interface IUserFollowedActivityRepository :  IRepository<UserFollowedActivity, long>
    {
        IList<UserFollowedActivity> Getactivities(int userId, bool? isActor, long beforeId, int maxResultCount);
    }
}