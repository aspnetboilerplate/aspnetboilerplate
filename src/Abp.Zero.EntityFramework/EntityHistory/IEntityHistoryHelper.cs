using System.Data.Entity;
using System.Threading.Tasks;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(DbContext context);

        Task SaveAsync(DbContext context, EntityChangeSet changeSet);

        void Save(DbContext context, EntityChangeSet changeSet);
    }
}
