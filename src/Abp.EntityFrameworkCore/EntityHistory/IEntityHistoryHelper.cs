using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false);

        Task SaveAsync(EntityEntry entityEntry);
    }
}
