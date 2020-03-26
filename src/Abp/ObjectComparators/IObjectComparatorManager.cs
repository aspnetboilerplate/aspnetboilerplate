using System;
using System.Collections.Immutable;

namespace Abp.ObjectComparators
{
    public interface IObjectComparatorManager
    {
        bool HasComparator<TBaseType>();

        ImmutableList<string> GetAllCompareTypes<TBaseType>();

        bool CanCompare<TBaseType>(string compareType);

        bool CanCompare<TBaseType, TEnumCompareType>(TEnumCompareType compareType) 
            where TEnumCompareType : Enum;

        bool Compare<TBaseType>(TBaseType baseObject, TBaseType compareObject, string compareType);

        bool Compare<TBaseType, TEnumCompareType>(TBaseType baseObject, TBaseType compareObject, TEnumCompareType compareType) 
            where TEnumCompareType : Enum;

        bool Compare<TBaseType>(TBaseType baseObject, ObjectComparatorCondition<TBaseType> condition);

        bool Compare<TBaseType, TEnumCompareType>(TBaseType baseObject, ObjectComparatorCondition<TBaseType, TEnumCompareType> condition)
            where TEnumCompareType : Enum;
    }
}
