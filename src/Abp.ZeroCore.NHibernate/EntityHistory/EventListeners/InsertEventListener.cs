using NHibernate.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.EntityHistory.EventListeners;

public class InsertEventListener : IPreInsertEventListener
{
    private readonly IEntityHistoryHelper _entityHistoryHelper;

    public InsertEventListener(IEntityHistoryHelper entityHistoryHelper)
    {
        _entityHistoryHelper = entityHistoryHelper;
    }

    public async Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return await Task.FromResult(false);
    }

    public bool OnPreInsert(PreInsertEvent @event)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return false;
    }
}