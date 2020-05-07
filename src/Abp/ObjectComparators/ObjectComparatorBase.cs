using System;
using System.Collections.Immutable;
using Abp.Dependency;

namespace Abp.ObjectComparators
{
    public abstract class ObjectComparatorBase : IObjectComparator, ITransientDependency
    {
        public abstract Type ObjectType { get; }

        public abstract ImmutableList<string> CompareTypes { get; }

        public abstract bool Compare(object baseObject, object compareObject, string compareType);

        public abstract bool CanCompare(Type baseObjectType, string compareType);
    }

    public abstract class ObjectComparatorBase<TBaseType> : ObjectComparatorBase
    {
        public sealed override Type ObjectType => typeof(TBaseType);

        private readonly bool _isNullable = false;

        protected ObjectComparatorBase()
        {
            _isNullable = IsNullableType(typeof(TBaseType));
        }

        protected abstract bool Compare(TBaseType baseObject, TBaseType compareObject, string compareType);

        public sealed override bool Compare(object baseObject, object compareObject, string compareType)
        {
            if (!_isNullable && (baseObject == null || compareObject == null))
            {
                throw new ArgumentNullException();
            }

            TBaseType baseObjTyped;
            TBaseType compareObjTyped;

            if (baseObject == null)
            {
                baseObjTyped = default;//which is null
            }
            else
            {
                baseObjTyped = (TBaseType)baseObject;
            }

            if (compareObject == null)
            {
                compareObjTyped = default;//which is null
            }
            else
            {
                compareObjTyped = (TBaseType)compareObject;
            }

            return Compare(baseObjTyped, compareObjTyped, compareType);
        }

        protected virtual bool CanCompare(string compareType)
        {
            return CompareTypes.Contains(compareType);
        }

        public sealed override bool CanCompare(Type baseObjectType, string compareType)
        {
            return _isNullable == IsNullableType(baseObjectType) && baseObjectType.IsAssignableFrom(typeof(TBaseType)) && CanCompare(compareType);
        }

        protected static bool IsNullableType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            return type == typeof(string);
        }
    }

    public abstract class ObjectComparatorBase<TBaseType, TEnumCompareTypes> : ObjectComparatorBase<TBaseType>
        where TEnumCompareTypes : Enum
    {
        public override ImmutableList<string> CompareTypes { get; }

        protected ObjectComparatorBase()
        {
            CompareTypes = Enum.GetNames(typeof(TEnumCompareTypes)).ToImmutableList();
        }

        protected abstract bool Compare(TBaseType baseObject, TBaseType compareObject, TEnumCompareTypes compareType);

        protected sealed override bool Compare(TBaseType baseObject, TBaseType compareObject, string compareType)
        {
            var compareTypeEnum = (TEnumCompareTypes)Enum.Parse(typeof(TEnumCompareTypes), compareType);
            return Compare(baseObject, compareObject, compareTypeEnum);
        }
    }
}
