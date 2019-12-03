using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Abp.EntityHistory.Extensions
{
    internal static class PropertyEntryExtensions
    {
        internal static object GetNewValue(this PropertyEntry propertyEntry)
        {
            if (propertyEntry.EntityEntry.State == EntityState.Deleted)
            {
                return null;
            }

            return propertyEntry.CurrentValue;
        }

        internal static object GetOriginalValue(this PropertyEntry propertyEntry)
        {
            if (propertyEntry.EntityEntry.State == EntityState.Added)
            {
                return null;
            }

            return propertyEntry.OriginalValue;
        }
    }
}