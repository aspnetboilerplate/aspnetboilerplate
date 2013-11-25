using Abp.Domain.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Data.Repositories
{
    public interface IActivityRepository : IRepository<Activity, long>
    {
        
    }
}