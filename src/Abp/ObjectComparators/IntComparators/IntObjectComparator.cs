using System;

namespace Abp.ObjectComparators.IntComparators
{
    public class IntObjectComparator : ObjectComparatorBase<int, IntCompareTypes>
    {
        protected override bool Compare(int baseObject, int compareObject, IntCompareTypes compareType)
        {
            switch (compareType)
            {
                case IntCompareTypes.Equals:
                    return baseObject == compareObject;
                case IntCompareTypes.LessThan:
                    return baseObject < compareObject;
                case IntCompareTypes.LessOrEqualThan:
                    return baseObject <= compareObject;
                case IntCompareTypes.BiggerThan:
                    return baseObject > compareObject;
                case IntCompareTypes.BiggerOrEqualThan:
                    return baseObject >= compareObject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    public class NullableIntObjectComparator : ObjectComparatorBase<int?, NullableIntCompareTypes>
    {
        protected override bool Compare(int? baseObject, int? compareObject, NullableIntCompareTypes compareType)
        {
            switch (compareType)
            {
                case NullableIntCompareTypes.Equals:
                    return baseObject == compareObject;
                case NullableIntCompareTypes.LessThan:
                    return baseObject < compareObject;
                case NullableIntCompareTypes.LessOrEqualThan:
                    return baseObject <= compareObject;
                case NullableIntCompareTypes.BiggerThan:
                    return baseObject > compareObject;
                case NullableIntCompareTypes.BiggerOrEqualThan:
                    return baseObject >= compareObject;
                case NullableIntCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableIntCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
