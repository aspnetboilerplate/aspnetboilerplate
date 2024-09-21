using System;
using NHibernate.Event;

namespace Abp.EntityHistory;

public interface IEntityHistoryHelper
{
    void AddEntityToChangeSet(AbstractPreDatabaseOperationEvent @event);
    void SaveChangeSet(Guid sessionId);
}