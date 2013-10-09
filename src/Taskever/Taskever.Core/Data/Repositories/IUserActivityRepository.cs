using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories
{
    public interface IUserActivityRepository :  IRepository<UserActivity, long>
    {
        
    }
}