using System;
using System.Linq;
using Abp.ObjectComparators;
using Abp.ObjectComparators.BooleanComparators;
using Abp.ObjectComparators.DateTimeComparators;
using Abp.ObjectComparators.IntComparators;
using Abp.ObjectComparators.LongComparators;
using Abp.ObjectComparators.StringComparators;
using Shouldly;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public abstract class ObjectComparatorTestBase<TObjectType, TEnumCompareTypes> : TestBaseWithLocalIocManager
        where TEnumCompareTypes : Enum
    {
        protected readonly IObjectComparatorManager ObjectComparatorManager;

        protected ObjectComparatorTestBase()
        {
            LocalIocManager.Register<IObjectComparator, IntObjectComparator>();
            LocalIocManager.Register<IObjectComparator, NullableIntObjectComparator>();

            LocalIocManager.Register<IObjectComparator, BooleanObjectComparator>();
            LocalIocManager.Register<IObjectComparator, NullableBooleanObjectComparator>();

            LocalIocManager.Register<IObjectComparator, LongObjectComparator>();
            LocalIocManager.Register<IObjectComparator, NullableLongObjectComparator>();

            LocalIocManager.Register<IObjectComparator, DateTimeObjectComparator>();
            LocalIocManager.Register<IObjectComparator, NullableDateTimeObjectComparator>();

            LocalIocManager.Register<IObjectComparator, StringObjectComparator>();
            LocalIocManager.Register<IObjectComparatorManager, ObjectComparatorManager>();

            ObjectComparatorManager = LocalIocManager.Resolve<IObjectComparatorManager>();
        }

        [Fact]
        public void Should_Get_All_Compare_Types()
        {
            var compareTypes = ObjectComparatorManager.GetAllCompareTypes<TObjectType>();
            compareTypes.ToList().SequenceEqual(Enum.GetNames(typeof(TEnumCompareTypes))).ShouldBeTrue();
        }

        [Fact]
        public void Should_Find_Comparator()
        {
            ObjectComparatorManager.HasComparator<TObjectType>().ShouldBeTrue();
        }

        [Fact]
        public void Can_Comparator_Tests()
        {
            foreach (var compareType in Enum.GetNames(typeof(TEnumCompareTypes)))
            {
                ObjectComparatorManager.CanCompare<TObjectType>(compareType).ShouldBeTrue();
            }

            ObjectComparatorManager.CanCompare<TObjectType>("test").ShouldBeFalse();
        }

        public virtual void Should_Compare(TObjectType baseObject, TObjectType compareObject, string compareType, bool result)
        {
            ObjectComparatorManager.Compare<TObjectType>(baseObject, compareObject, compareType).ShouldBe(result);
        }

        public virtual void Should_Compare_With_ObjectComparatorCondition(TObjectType baseObject, TObjectType compareObject, string compareType, bool result)
        {
            var condition = new ObjectComparatorCondition<TObjectType>();
            condition.SetValue(compareObject);
            condition.CompareType = compareType;

            ObjectComparatorManager.Compare<TObjectType>(baseObject, condition).ShouldBe(result);
        }
    }
}

