using NHibernate.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.EntityHistory.EventListeners;

public class DeleteEventListener : IPreDeleteEventListener
{
    private readonly IEntityHistoryHelper _entityHistoryHelper;

    public DeleteEventListener(IEntityHistoryHelper entityHistoryHelper)
    {
        _entityHistoryHelper = entityHistoryHelper;
    }

    public async Task<bool> OnPreDeleteAsync(PreDeleteEvent @event, CancellationToken cancellationToken)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return await Task.FromResult(false);
    }

    public bool OnPreDelete(PreDeleteEvent @event)
    {
        _entityHistoryHelper.AddEntityToChangeSet(@event);
        return false;
    }
}