using System.Collections.Generic;
using Abp.ObjectComparators.BooleanComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class NullableBooleanComparators_Tests : ObjectComparatorTestBase<bool?, NullableBooleanCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { true, true, NullableBooleanCompareTypes.Equals.ToString(), true };
            yield return new object[] { true, false, NullableBooleanCompareTypes.Equals.ToString(), false };
            yield return new object[] { false, true, NullableBooleanCompareTypes.Equals.ToString(), false };
            yield return new object[] { false, false, NullableBooleanCompareTypes.Equals.ToString(), true };

            yield return new object[] { null, false, NullableBooleanCompareTypes.Null.ToString(), true };
            yield return new object[] { null, true, NullableBooleanCompareTypes.Null.ToString(), true };
            yield return new object[] { true, null, NullableBooleanCompareTypes.Null.ToString(), false };
            yield return new object[] { false, null, NullableBooleanCompareTypes.Null.ToString(), false };

            yield return new object[] { null, false, NullableBooleanCompareTypes.NotNull.ToString(), false };
            yield return new object[] { false, null, NullableBooleanCompareTypes.NotNull.ToString(), true };
            yield return new object[] { true, false, NullableBooleanCompareTypes.NotNull.ToString(), true };
            yield return new object[] { false, false, NullableBooleanCompareTypes.NotNull.ToString(), true };
            yield return new object[] { false, true, NullableBooleanCompareTypes.NotNull.ToString(), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(bool? baseObject, bool? compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(bool? baseObject, bool? compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
