using System.Collections.Generic;
using Abp.ObjectComparators.IntComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class NullableIntObjectComparator_Tests : ObjectComparatorTestBase<int?, NullableIntCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { 1, 0, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 1, -1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 0, 1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 0, -1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { -1, 1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 1, 1, nameof(NullableIntCompareTypes.Equals), true };
            yield return new object[] { -1, -1, nameof(NullableIntCompareTypes.Equals), true };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.Equals), true };
            yield return new object[] { null, null, nameof(NullableIntCompareTypes.Equals), true };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.Equals), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.Equals), false };

            yield return new object[] { 1, 0, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(NullableIntCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(NullableIntCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { -1, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { null, null, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.LessThan), false };

            yield return new object[] { 1, 0, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(NullableIntCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(NullableIntCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(NullableIntCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(NullableIntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(NullableIntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { null, null, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.LessOrEqualThan), false };

            yield return new object[] { 1, 0, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { -1, -1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { null, null, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.BiggerThan), false };

            yield return new object[] { 1, 0, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(NullableIntCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(NullableIntCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(NullableIntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(NullableIntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { null, null, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.BiggerOrEqualThan), false };

            yield return new object[] { null, null, nameof(NullableIntCompareTypes.Null), true };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.Null), true };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.Null), true };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.Null), true };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.Null), false };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.Null), false };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.Null), false };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.Null), false };

            yield return new object[] { null, null, nameof(NullableIntCompareTypes.NotNull), false };
            yield return new object[] { null, 1, nameof(NullableIntCompareTypes.NotNull), false };
            yield return new object[] { null, -1, nameof(NullableIntCompareTypes.NotNull), false };
            yield return new object[] { null, 0, nameof(NullableIntCompareTypes.NotNull), false };
            yield return new object[] { 1, null, nameof(NullableIntCompareTypes.NotNull), true };
            yield return new object[] { -1, null, nameof(NullableIntCompareTypes.NotNull), true };
            yield return new object[] { 0, null, nameof(NullableIntCompareTypes.NotNull), true };
            yield return new object[] { 0, 0, nameof(NullableIntCompareTypes.NotNull), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(int? baseObject, int? compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }


        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(int? baseObject, int? compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
