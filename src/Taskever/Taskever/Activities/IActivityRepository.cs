using Abp.Domain.Repositories;

namespace Taskever.Activities
{
    public interface IActivityRepository : IRepository<Activity, long>
    {
        
    }
}