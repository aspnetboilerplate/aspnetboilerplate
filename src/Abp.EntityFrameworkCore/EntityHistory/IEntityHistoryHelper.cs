using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);

        bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false);

        void Save(EntityChangeSet changeSet, DbContext context);

        Task SaveAsync(EntityChangeSet changeSet, DbContext context);

        Task SaveAsync(EntityEntry entityEntry);
    }
}
