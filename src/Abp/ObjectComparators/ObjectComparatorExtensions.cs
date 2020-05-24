using Abp.Extensions;
using System;

namespace Abp.ObjectComparators
{
    public static class ObjectComparatorExtensions
    {
        public static bool IsNullOrEmpty<T1, T2>(this ObjectComparatorCondition<T1, T2> objectComparator) where T2 : Enum
        {
            return objectComparator == null;
        }

        public static bool IsNullOrEmpty<T>(this ObjectComparatorCondition<T> objectComparator)
        {
            return objectComparator == null || objectComparator.CompareType.IsNullOrWhiteSpace();
        }
    }
}
