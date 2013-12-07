using Abp.Domain.Repositories;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Domain.Repositories
{
    public interface IActivityRepository : IRepository<Activity, long>
    {
        
    }
}