using System.Collections.Generic;
using Abp.ObjectComparators.LongComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class NullableLongObjectComparator_Tests : ObjectComparatorTestBase<long?, NullableLongCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { 1, 0, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 1, -1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 0, 1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 0, -1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { -1, 1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 1, 1, nameof(NullableLongCompareTypes.Equals), true };
            yield return new object[] { -1, -1, nameof(NullableLongCompareTypes.Equals), true };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.Equals), true };
            yield return new object[] { null, null, nameof(NullableLongCompareTypes.Equals), true };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.Equals), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.Equals), false };

            yield return new object[] { 1, 0, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(NullableLongCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(NullableLongCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { -1, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { null, null, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.LessThan), false };

            yield return new object[] { 1, 0, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 1, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { 0, 1, nameof(NullableLongCompareTypes.LessThan), true };
            yield return new object[] { 0, -1, nameof(NullableLongCompareTypes.LessThan), false };
            yield return new object[] { -1, 1, nameof(NullableLongCompareTypes.LessThan), true };
            yield return new object[] { 1, 1, nameof(NullableLongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(NullableLongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.LessOrEqualThan), true };
            yield return new object[] { null, null, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.LessOrEqualThan), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.LessOrEqualThan), false };

            yield return new object[] { 1, 0, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { -1, -1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { null, null, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.BiggerThan), false };

            yield return new object[] { 1, 0, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { 1, -1, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { 0, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 0, -1, nameof(NullableLongCompareTypes.BiggerThan), true };
            yield return new object[] { -1, 1, nameof(NullableLongCompareTypes.BiggerThan), false };
            yield return new object[] { 1, 1, nameof(NullableLongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { -1, -1, nameof(NullableLongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { null, null, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.BiggerOrEqualThan), false };

            yield return new object[] { null, null, nameof(NullableLongCompareTypes.Null), true };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.Null), true };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.Null), true };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.Null), true };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.Null), false };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.Null), false };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.Null), false };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.Null), false };

            yield return new object[] { null, null, nameof(NullableLongCompareTypes.NotNull), false };
            yield return new object[] { null, 1, nameof(NullableLongCompareTypes.NotNull), false };
            yield return new object[] { null, -1, nameof(NullableLongCompareTypes.NotNull), false };
            yield return new object[] { null, 0, nameof(NullableLongCompareTypes.NotNull), false };
            yield return new object[] { 1, null, nameof(NullableLongCompareTypes.NotNull), true };
            yield return new object[] { -1, null, nameof(NullableLongCompareTypes.NotNull), true };
            yield return new object[] { 0, null, nameof(NullableLongCompareTypes.NotNull), true };
            yield return new object[] { 0, 0, nameof(NullableLongCompareTypes.NotNull), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(long? baseObject, long? compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(long? baseObject, long? compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
