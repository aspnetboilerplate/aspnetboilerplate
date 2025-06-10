using NHibernate.Event;
using System;
using System.Linq;

namespace Abp.EntityHistory;

public static class EventExtensions
{
    public static bool IsDeleted(this PreUpdateEvent @event)
    {
        var dirtyFieldIndexes = @event.Persister.FindDirty(
            @event.State,
            @event.OldState,
            @event.Entity,
            @event.Session
        );

        foreach (var dirtyFieldIndex in dirtyFieldIndexes)
        {
            var dirtyFieldProperty = @event.Persister.EntityMetamodel.Type.GetProperties()
                .FirstOrDefault(p => p.Name == @event.Persister.PropertyNames[dirtyFieldIndex]);

            if (dirtyFieldProperty == null ||
                !dirtyFieldProperty.Name.Equals("IsDeleted", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var newValue = @event.State[dirtyFieldIndex]?.ToString() ?? string.Empty;
            var oldValue = @event.OldState[dirtyFieldIndex]?.ToString() ?? string.Empty;

            if (oldValue.Equals(newValue, StringComparison.InvariantCultureIgnoreCase) ||
                !newValue.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            return true;
        }

        return false;
    }
}