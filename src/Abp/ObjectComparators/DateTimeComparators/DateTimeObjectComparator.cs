using System;

namespace Abp.ObjectComparators.DateTimeComparators
{
    public class DateTimeObjectComparator : ObjectComparatorBase<DateTime, DateTimeCompareTypes>
    {
        protected override bool Compare(DateTime baseObject, DateTime compareObject, DateTimeCompareTypes compareType)
        {
            switch (compareType)
            {
                case DateTimeCompareTypes.Equals:
                    return baseObject.Equals(compareObject);
                case DateTimeCompareTypes.LessThan:
                    return baseObject.CompareTo(compareObject) < 0;
                case DateTimeCompareTypes.LessOrEqualThan:
                    return baseObject.CompareTo(compareObject) <= 0;
                case DateTimeCompareTypes.BiggerThan:
                    return baseObject.CompareTo(compareObject) > 0;
                case DateTimeCompareTypes.BiggerOrEqualThan:
                    return baseObject.CompareTo(compareObject) >= 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    public class NullableDateTimeObjectComparator : ObjectComparatorBase<DateTime?, NullableDateTimeCompareTypes>
    {
        protected override bool Compare(DateTime? baseObject, DateTime? compareObject, NullableDateTimeCompareTypes compareType)
        {
            var conditionBothHasValue = baseObject.HasValue && compareObject.HasValue;
            switch (compareType)
            {
                case NullableDateTimeCompareTypes.Equals:
                    return baseObject.Equals(compareObject);
                case NullableDateTimeCompareTypes.LessThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) < 0;
                case NullableDateTimeCompareTypes.LessOrEqualThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) <= 0;
                case NullableDateTimeCompareTypes.BiggerThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) > 0;
                case NullableDateTimeCompareTypes.BiggerOrEqualThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) >= 0;
                case NullableDateTimeCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableDateTimeCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
