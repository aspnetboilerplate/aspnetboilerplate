using System;
using Castle.Core.Internal;

namespace Abp.ObjectComparators.StringComparators
{
    public class StringObjectComparator : ObjectComparatorBase<string, StringCompareTypes>
    {
        protected override bool Compare(string baseObject, string compareObject, StringCompareTypes compareTypes)
        {
            switch (compareTypes)
            {
                case StringCompareTypes.Equals:
                    return baseObject == compareObject;
                case StringCompareTypes.Contains:
                    return baseObject.Contains(compareObject);
                case StringCompareTypes.StartsWith:
                    return baseObject.StartsWith(compareObject);
                case StringCompareTypes.EndsWith:
                    return baseObject.EndsWith(compareObject);
                case StringCompareTypes.Null:
                    return baseObject.IsNullOrEmpty();
                case StringCompareTypes.NotNull:
                    return !baseObject.IsNullOrEmpty();
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareTypes), compareTypes, null);
            }
        }
    }
}
