using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace Abp.EntityHistory.Extensions
{
    internal static class DbComplexPropertyEntryExtensions
    {
        internal static bool HasChanged(this DbComplexPropertyEntry propertyEntry, EdmProperty complexProperty)
        {
            return HasModifications(complexProperty, propertyEntry.GetNewValue(), propertyEntry.GetOriginalValue());
        }

        private static bool HasModifications(EdmProperty complexTypeProperty, object newValue, object originalValue)
        {
            if (!complexTypeProperty.IsComplexType)
            {
                return false;
            }

            if (newValue == null && originalValue == null)
            {
                return false;
            }

            var isModified = false;
            foreach (var property in complexTypeProperty.ComplexType.Properties)
            {
                var propertyNewValue = newValue?.GetType()?.GetProperty(property.Name)?.GetValue(newValue);
                var propertyOldValue = originalValue?.GetType()?.GetProperty(property.Name)?.GetValue(originalValue);
                if (property.IsPrimitiveType)
                {
                    isModified = !(propertyOldValue?.Equals(propertyNewValue) ?? propertyNewValue == null);
                }
                else if (property.IsComplexType)
                {
                    isModified = HasModifications(property, propertyNewValue, propertyOldValue);
                }

                if (isModified)
                {
                    break;
                }
            }
            return isModified;
        }
    }
}