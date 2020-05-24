using System;

namespace Abp.ObjectComparators.BooleanComparators
{
    public class BooleanObjectComparator : ObjectComparatorBase<bool, BooleanCompareTypes>
    {
        protected override bool Compare(bool baseObject, bool compareObject, BooleanCompareTypes compareType)
        {
            switch (compareType)
            {
                case BooleanCompareTypes.Equals:
                    return baseObject == compareObject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    public class NullableBooleanObjectComparator : ObjectComparatorBase<bool?, NullableBooleanCompareTypes>
    {
        protected override bool Compare(bool? baseObject, bool? compareObject, NullableBooleanCompareTypes compareType)
        {
            switch (compareType)
            {
                case NullableBooleanCompareTypes.Equals:
                    return baseObject == compareObject;
                case NullableBooleanCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableBooleanCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
