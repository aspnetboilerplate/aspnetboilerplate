using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities.EventHistories;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhEventHistoryRepository : NhRepositoryBase<EventHistory, long>, IEventHistoryRepository
    {

    }
}