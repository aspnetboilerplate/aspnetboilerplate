using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.ObjectComparators;
using Abp.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class MyTestStringObjectComparator : ObjectComparatorBase<string>
    {
        public const string EqualsCompareType = "EqualsCompareType";
        public const string ReverseOfSecondIsEqualtoFirstCompareType = "ReverseOfSecondIsEqualtoFirst";

        public override ImmutableList<string> CompareTypes { get; }

        public MyTestStringObjectComparator()
        {
            CompareTypes = ImmutableList.Create(EqualsCompareType, ReverseOfSecondIsEqualtoFirstCompareType);
        }

        protected override bool Compare(string baseObject, string compareObject, string compareType)
        {
            if (compareType != ReverseOfSecondIsEqualtoFirstCompareType)
            {
                return baseObject == compareObject;
            }
            
            var array = compareObject.ToCharArray();
            Array.Reverse(array);
            return baseObject == new string(array);

        }
    }

    public class ObjectComparatorTestClass
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }

        public ObjectComparatorTestClass(string prop1, string prop2)
        {
            Prop1 = prop1;
            Prop2 = prop2;
        }
    }

    public enum ObjectComparatorTestClassCompareTypes
    {
        Equals, FirstProp1BiggerThanSecondProp2AsInt//so, whatever you want
    }

    public class ObjectComparatorTestClassObjectComparator : ObjectComparatorBase<ObjectComparatorTestClass, ObjectComparatorTestClassCompareTypes>// you can create comparator for anything
    {
        protected override bool Compare(ObjectComparatorTestClass baseObject, ObjectComparatorTestClass compareObject,
            ObjectComparatorTestClassCompareTypes compareType)
        {
            if (baseObject == null && compareObject == null)
            {
                return true;
            }

            if (baseObject == null || compareObject == null)
            {
                return false;
            }

            switch (compareType)
            {
                case ObjectComparatorTestClassCompareTypes.Equals:
                    return baseObject.Prop1.Equals(compareObject.Prop1) && baseObject.Prop2.Equals(compareObject.Prop2);
                case ObjectComparatorTestClassCompareTypes.FirstProp1BiggerThanSecondProp2AsInt:
                    return int.Parse(baseObject.Prop1) > int.Parse(compareObject.Prop2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    public class ObjectComparatorManager_Tests : TestBaseWithLocalIocManager
    {
        private readonly IObjectComparatorManager _objectComparatorManager;

        public ObjectComparatorManager_Tests()
        {
            LocalIocManager.Register<IObjectComparator, ObjectComparatorTestClassObjectComparator>();
            LocalIocManager.Register<IObjectComparator, MyTestStringObjectComparator>();
            LocalIocManager.Register<IObjectComparatorManager, ObjectComparatorManager>();

            _objectComparatorManager = LocalIocManager.Resolve<IObjectComparatorManager>();
        }

        [Fact]
        public void Should_Get_All_Compare_Types_By_Type()
        {
            var compareTypesForString = _objectComparatorManager.GetAllCompareTypes<string>();
            compareTypesForString.ShouldContain(MyTestStringObjectComparator.EqualsCompareType);
            compareTypesForString.ShouldContain(MyTestStringObjectComparator.ReverseOfSecondIsEqualtoFirstCompareType);

            var compareTypesForClass = _objectComparatorManager.GetAllCompareTypes<ObjectComparatorTestClass>();
            compareTypesForClass.SequenceEqual(Enum.GetNames(typeof(ObjectComparatorTestClassCompareTypes))).ShouldBeTrue();
        }

        [Fact]
        public void Should_Get_All_Compare_Types()
        {
            var compareTypes = _objectComparatorManager.GetAllCompareTypes();

            compareTypes.ContainsKey(typeof(ObjectComparatorTestClass)).ShouldBeTrue();
            var compareTypesForTestClass = compareTypes[typeof(ObjectComparatorTestClass)];
            compareTypesForTestClass.SequenceEqual(Enum.GetNames(typeof(ObjectComparatorTestClassCompareTypes))).ShouldBeTrue();

            compareTypes.ContainsKey(typeof(string)).ShouldBeTrue();
            var compareTypesForString = compareTypes[typeof(string)];
            compareTypesForString.ShouldContain(MyTestStringObjectComparator.EqualsCompareType);
            compareTypesForString.ShouldContain(MyTestStringObjectComparator.ReverseOfSecondIsEqualtoFirstCompareType);
        }

        [Fact]
        public void Should_Find_Comparator()
        {
            _objectComparatorManager.HasComparator<string>().ShouldBeTrue();
            _objectComparatorManager.HasComparator<ObjectComparatorTestClass>().ShouldBeTrue();
            _objectComparatorManager.HasComparator<bool>().ShouldBeFalse();
        }

        [Fact]
        public void Can_Comparator_Tests()
        {
            _objectComparatorManager.CanCompare<string>(MyTestStringObjectComparator.EqualsCompareType).ShouldBeTrue();
            _objectComparatorManager.CanCompare<string>(MyTestStringObjectComparator.ReverseOfSecondIsEqualtoFirstCompareType).ShouldBeTrue();
            _objectComparatorManager.CanCompare<string>("NonExistCompareType").ShouldBeFalse();

            _objectComparatorManager
                .CanCompare<ObjectComparatorTestClass, ObjectComparatorTestClassCompareTypes>(ObjectComparatorTestClassCompareTypes.Equals)
                .ShouldBeTrue();
            _objectComparatorManager
                .CanCompare<ObjectComparatorTestClass, ObjectComparatorTestClassCompareTypes>(ObjectComparatorTestClassCompareTypes.FirstProp1BiggerThanSecondProp2AsInt)
                .ShouldBeTrue();
            _objectComparatorManager.CanCompare<ObjectComparatorTestClass>("test").ShouldBeFalse();
        }

        public static IEnumerable<object[]> Should_Compare_String_Data_Generator()
        {
            yield return new object[] { "123", "321", MyTestStringObjectComparator.ReverseOfSecondIsEqualtoFirstCompareType, true };
            yield return new object[] { "123", "123", MyTestStringObjectComparator.ReverseOfSecondIsEqualtoFirstCompareType, false };
            yield return new object[] { "123", "123", MyTestStringObjectComparator.EqualsCompareType, true };
            yield return new object[] { "123", "321", MyTestStringObjectComparator.EqualsCompareType, false };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_String_Data_Generator))]
        public void Should_Compare_String(string baseObject, string compareObject, string compareType, bool result)
        {
            _objectComparatorManager.Compare(baseObject, compareObject, compareType).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_String_Data_Generator))]
        public void Should_Compare_String_With_ObjectComparatorCondition(string baseObject, string compareObject, string compareType, bool result)
        {
            var condition = new ObjectComparatorCondition<string>();
            condition.SetValue(compareObject);
            condition.CompareType = compareType;

            _objectComparatorManager.Compare(baseObject, condition).ShouldBe(result);
        }

        public static IEnumerable<object[]> Should_Compare_Test_Class_Data_Generator()
        {
            yield return new object[]
            {
                new ObjectComparatorTestClass("1","2"),
                new ObjectComparatorTestClass("1","2"),
                ObjectComparatorTestClassCompareTypes.Equals,
                true
            };

            yield return new object[]
            {
                new ObjectComparatorTestClass("1","2"),
                new ObjectComparatorTestClass("1123","test"),
                ObjectComparatorTestClassCompareTypes.Equals,
                false
            };

            yield return new object[]
            {
                new ObjectComparatorTestClass("5","2"),
                new ObjectComparatorTestClass("5","2"),
                ObjectComparatorTestClassCompareTypes.FirstProp1BiggerThanSecondProp2AsInt,
                true
            };

            yield return new object[]
            {
                new ObjectComparatorTestClass("2","5"),
                new ObjectComparatorTestClass("2","5"),
                ObjectComparatorTestClassCompareTypes.FirstProp1BiggerThanSecondProp2AsInt,
                false
            };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Test_Class_Data_Generator))]
        public void Should_Compare_Test_Class(
            ObjectComparatorTestClass baseObject,
            ObjectComparatorTestClass compareObject,
            ObjectComparatorTestClassCompareTypes compareType,
            bool result)
        {
            _objectComparatorManager.Compare(baseObject, compareObject, compareType).ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Test_Class_Data_Generator))]
        public void Should_Compare_Test_Class_With_ObjectComparatorCondition(
            ObjectComparatorTestClass baseObject,
            ObjectComparatorTestClass compareObject,
            ObjectComparatorTestClassCompareTypes compareType,
            bool result)
        {
            var condition = new ObjectComparatorCondition<ObjectComparatorTestClass, ObjectComparatorTestClassCompareTypes>();
            condition.SetValue(compareObject);
            condition.CompareType = compareType;

            _objectComparatorManager.Compare(baseObject, condition).ShouldBe(result);
        }
    }
}
