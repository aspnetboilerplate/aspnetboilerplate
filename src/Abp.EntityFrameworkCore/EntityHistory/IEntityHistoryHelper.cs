using Abp.Events.Bus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(EntityChangeReport changeReport);

        bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false);

        Task SaveAsync(EntityChangeSet changeSet, DbContext context);

        Task SaveAsync(EntityEntry entityEntry);
    }
}
