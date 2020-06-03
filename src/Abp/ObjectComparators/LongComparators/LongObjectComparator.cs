using System;

namespace Abp.ObjectComparators.LongComparators
{

    public class LongObjectComparator : ObjectComparatorBase<long, LongCompareTypes>
    {
        protected override bool Compare(long baseObject, long compareObject, LongCompareTypes compareType)
        {
            switch (compareType)
            {
                case LongCompareTypes.Equals:
                    return baseObject == compareObject;
                case LongCompareTypes.LessThan:
                    return baseObject < compareObject;
                case LongCompareTypes.LessOrEqualThan:
                    return baseObject <= compareObject;
                case LongCompareTypes.BiggerThan:
                    return baseObject > compareObject;
                case LongCompareTypes.BiggerOrEqualThan:
                    return baseObject >= compareObject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    public class NullableLongObjectComparator : ObjectComparatorBase<long?, NullableLongCompareTypes>
    {
        protected override bool Compare(long? baseObject, long? compareObject, NullableLongCompareTypes compareType)
        {
            switch (compareType)
            {
                case NullableLongCompareTypes.Equals:
                    return baseObject == compareObject;
                case NullableLongCompareTypes.LessThan:
                    return baseObject < compareObject;
                case NullableLongCompareTypes.LessOrEqualThan:
                    return baseObject <= compareObject;
                case NullableLongCompareTypes.BiggerThan:
                    return baseObject > compareObject;
                case NullableLongCompareTypes.BiggerOrEqualThan:
                    return baseObject >= compareObject;
                case NullableLongCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableLongCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
