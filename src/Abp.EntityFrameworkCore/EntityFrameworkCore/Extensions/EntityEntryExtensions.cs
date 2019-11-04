using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Abp.EntityFrameworkCore.Extensions
{
    public static class EntityEntryExtensions
    {
        /// <summary>
        /// Check if the entity and its associated Owned entity have changed.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool CheckOwnedEntityChange(this EntityEntry entry)
        {
            return entry.State == EntityState.Modified ||
                   entry.References.Any(r =>
                       r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && CheckOwnedEntityChange(r.TargetEntry));
        }
    }
}
