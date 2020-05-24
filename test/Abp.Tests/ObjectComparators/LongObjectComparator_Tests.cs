using System.Collections.Generic;
using Abp.ObjectComparators.LongComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class LongObjectComparator_Tests : ObjectComparatorTestBase<long, LongCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { 1, 0, nameof(LongCompareTypes.Equals), false };
            yield return new object[] { 1, -1, nameof(LongCompareTypes.Equals), false };
            yield return new object[] { 0, 1, nameof(LongCompareTypes.Equals), false };
            yield return new object[] { 0, -1, nameof(LongCompareTypes.Equals), false };
            yield return new object[] { -1, 1, nameof(LongCompareTypes.Equals), false };
            yield return new object[] { 1, 1, nameof(LongCompareTypes.Equals), true };
            yield return new object[] { -1, -1, nameof(LongCompareTypes.Equals), true };
            yield return new object[] { 0, 0, nameof(LongCompareTypes.Equals), true };

            yield return new object[] { 1, 0, nameof(LongCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(LongCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(LongCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(LongCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(LongCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(LongCompareTypes.LessThan), false };
            yield return new object[] { -1, -1, nameof(LongCompareTypes.LessThan), false };
            yield return new object[] { 0, 0, nameof(LongCompareTypes.LessThan), false };

            yield return new object[] { 1, 0, nameof(LongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 1, -1, nameof(LongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 0, 1, nameof(LongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, -1, nameof(LongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { -1, 1, nameof(LongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 1, 1, nameof(LongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(LongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(LongCompareTypes.LessOrEqualThan), true };

            yield return new object[] { 1, 0, nameof(LongCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(LongCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(LongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(LongCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(LongCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(LongCompareTypes.BiggerThan), false };
            yield return new object[] { -1, -1, nameof(LongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, 0, nameof(LongCompareTypes.BiggerThan), false };

            yield return new object[] { 1, 0, nameof(LongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 1, -1, nameof(LongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 1, nameof(LongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 0, -1, nameof(LongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, 1, nameof(LongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 1, 1, nameof(LongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(LongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(LongCompareTypes.BiggerOrEqualThan), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(long baseObject, long compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(long baseObject, long compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
