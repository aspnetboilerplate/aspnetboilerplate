using NHibernate.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.EntityHistory.EventListeners;

public class UpdateEventListener : IPreUpdateEventListener
{
    private readonly IEntityHistoryHelper _entityHistoryHelper;

    public UpdateEventListener(IEntityHistoryHelper entityHistoryHelper)
    {
        _entityHistoryHelper = entityHistoryHelper;
    }
    public async Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return await Task.FromResult(false);
    }

    public bool OnPreUpdate(PreUpdateEvent @event)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return false;
    }
}