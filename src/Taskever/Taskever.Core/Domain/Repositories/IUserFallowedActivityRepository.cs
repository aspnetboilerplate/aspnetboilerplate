using System.Linq;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Repositories
{
    public interface IUserFollowedActivityRepository :  IRepository<UserFollowedActivity, long>
    {
        IQueryable<UserFollowedActivity> GetAllWithActivity();
    }
}