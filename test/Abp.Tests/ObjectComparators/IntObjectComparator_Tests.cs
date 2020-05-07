using System.Collections.Generic;
using Abp.ObjectComparators.IntComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class IntObjectComparator_Tests : ObjectComparatorTestBase<int, IntCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { 1, 0, nameof(IntCompareTypes.Equals), false };
            yield return new object[] { 1, -1, nameof(IntCompareTypes.Equals), false };
            yield return new object[] { 0, 1, nameof(IntCompareTypes.Equals), false };
            yield return new object[] { 0, -1, nameof(IntCompareTypes.Equals), false };
            yield return new object[] { -1, 1, nameof(IntCompareTypes.Equals), false };
            yield return new object[] { 1, 1, nameof(IntCompareTypes.Equals), true };
            yield return new object[] { -1, -1, nameof(IntCompareTypes.Equals), true };
            yield return new object[] { 0, 0, nameof(IntCompareTypes.Equals), true };

            yield return new object[] { 1, 0, nameof(IntCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(IntCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(IntCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(IntCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(IntCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(IntCompareTypes.LessThan), false };
            yield return new object[] { -1, -1, nameof(IntCompareTypes.LessThan), false };
            yield return new object[] { 0, 0, nameof(IntCompareTypes.LessThan), false };

            yield return new object[] { 1, 0, nameof(IntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 1, -1, nameof(IntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 0, 1, nameof(IntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, -1, nameof(IntCompareTypes.LessOrEqualThan), false };
            yield return new object[] { -1, 1, nameof(IntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 1, 1, nameof(IntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(IntCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(IntCompareTypes.LessOrEqualThan), true };

            yield return new object[] { 1, 0, nameof(IntCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(IntCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(IntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(IntCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(IntCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(IntCompareTypes.BiggerThan), false };
            yield return new object[] { -1, -1, nameof(IntCompareTypes.BiggerThan), false };
            yield return new object[] { 0, 0, nameof(IntCompareTypes.BiggerThan), false };

            yield return new object[] { 1, 0, nameof(IntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 1, -1, nameof(IntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 1, nameof(IntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 0, -1, nameof(IntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, 1, nameof(IntCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 1, 1, nameof(IntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(IntCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(IntCompareTypes.BiggerOrEqualThan), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(int baseObject, int compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(int baseObject, int compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
