using Abp.ObjectComparators.BooleanComparators;
using System.Collections.Generic;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class BooleanComparators_Tests : ObjectComparatorTestBase<bool, BooleanCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { true, true, BooleanCompareTypes.Equals.ToString(), true };
            yield return new object[] { true, false, BooleanCompareTypes.Equals.ToString(), false };
            yield return new object[] { false, true, BooleanCompareTypes.Equals.ToString(), false };
            yield return new object[] { false, false, BooleanCompareTypes.Equals.ToString(), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(bool baseObject, bool compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(bool baseObject, bool compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
