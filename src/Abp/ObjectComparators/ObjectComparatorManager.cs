using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Dependency;

namespace Abp.ObjectComparators
{
    public class ObjectComparatorManager : IObjectComparatorManager, ITransientDependency
    {
        private readonly IEnumerable<IObjectComparator> _objectComparators;

        public ObjectComparatorManager(IocManager iocManager)
        {
            _objectComparators = iocManager.ResolveAll<IObjectComparator>();
        }

        public bool HasComparator<TBaseType>()
        {
            return _objectComparators.Any(comparator => comparator.ObjectType == typeof(TBaseType));
        }

        public ImmutableList<string> GetAllCompareTypes<TBaseType>()
        {
            return _objectComparators
                .Where(comparator => comparator.ObjectType == typeof(TBaseType))
                .SelectMany(comparator => comparator.CompareTypes).Distinct().ToImmutableList();
        }

        public Dictionary<Type, List<string>> GetAllCompareTypes()
        {
            return _objectComparators
                .GroupBy(compareType => compareType.ObjectType)
                .Select(comparator =>
                    new
                    {
                        ObjectType = comparator.Key,
                        CompareTypes = comparator.SelectMany(c => c.CompareTypes).Distinct().ToList()
                    })
                .ToDictionary(x => x.ObjectType, y => y.CompareTypes);
        }

        public bool CanCompare<TBaseType>(string compareType)
        {
            return _objectComparators.Any(objectComparator => objectComparator.CanCompare(typeof(TBaseType), compareType));
        }

        public bool CanCompare<TBaseType, TEnumCompareType>(TEnumCompareType compareType) where TEnumCompareType : Enum
        {
            return CanCompare<TBaseType>(compareType.ToString());
        }

        public bool Compare<TBaseType>(TBaseType baseObject, TBaseType compareObject, string compareType)
        {
            foreach (var objectComparator in _objectComparators)
            {
                if (objectComparator.CanCompare(typeof(TBaseType), compareType))
                {
                    return objectComparator.Compare(baseObject, compareObject, compareType);
                }
            }

            throw new KeyNotFoundException($"There is no comparator with {typeof(TBaseType).Name} base type and {compareType} compare type");
        }

        public bool Compare<TBaseType, TEnumCompareType>(TBaseType baseObject, TBaseType compareObject, TEnumCompareType compareType)
            where TEnumCompareType : Enum
        {
            return Compare<TBaseType>(baseObject, compareObject, compareType.ToString());
        }

        public bool Compare<TBaseType>(TBaseType baseObject, ObjectComparatorCondition<TBaseType> condition)
        {
            foreach (var objectComparator in _objectComparators)
            {
                if (objectComparator.CanCompare(typeof(TBaseType), condition.CompareType))
                {
                    return objectComparator.Compare(baseObject, condition.GetValue(), condition.CompareType);
                }
            }

            throw new KeyNotFoundException($"There is no comparator with {typeof(TBaseType).Name} base type and {condition.CompareType} compare type");
        }

        public bool Compare<TBaseType, TEnumCompareType>(TBaseType baseObject, ObjectComparatorCondition<TBaseType, TEnumCompareType> condition) where TEnumCompareType : Enum
        {
            foreach (var objectComparator in _objectComparators)
            {
                if (objectComparator.CanCompare(typeof(TBaseType), condition.CompareType.ToString()))
                {
                    return objectComparator.Compare(baseObject, condition.GetValue(), condition.CompareType.ToString());
                }
            }

            throw new KeyNotFoundException($"There is no comparator with {typeof(TBaseType).Name} base type and {condition.CompareType} compare type");
        }
    }
}
