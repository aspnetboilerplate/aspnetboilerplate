using Abp.Domain.Repositories;
using Taskever.Domain.Entities.EventHistories;

namespace Taskever.Data.Repositories
{
    public interface IEventHistoryRepository : IRepository<EventHistory, long>
    {
        
    }
}