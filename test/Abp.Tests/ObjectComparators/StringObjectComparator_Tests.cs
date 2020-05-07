using System.Collections.Generic;
using Abp.ObjectComparators.StringComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class StringObjectComparator_Tests : ObjectComparatorTestBase<string, StringCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { "", "", StringCompareTypes.Equals.ToString(), true };
            yield return new object[] { "test", "", StringCompareTypes.Equals.ToString(), false };
            yield return new object[] { "", "test", StringCompareTypes.Equals.ToString(), false };
            yield return new object[] { "", "test", StringCompareTypes.Equals.ToString(), false };

            yield return new object[] { "test123", "test", StringCompareTypes.Contains.ToString(), true };
            yield return new object[] { "test", "test123", StringCompareTypes.Contains.ToString(), false };
            yield return new object[] { "test", "", StringCompareTypes.Contains.ToString(), true };

            yield return new object[] { "test123", "test", StringCompareTypes.StartsWith.ToString(), true };
            yield return new object[] { "test", "test123", StringCompareTypes.StartsWith.ToString(), false };
            yield return new object[] { "test", "", StringCompareTypes.StartsWith.ToString(), true };

            yield return new object[] { "test123", "123", StringCompareTypes.EndsWith.ToString(), true };
            yield return new object[] { "test123", "test", StringCompareTypes.EndsWith.ToString(), false };
            yield return new object[] { "test123", "", StringCompareTypes.EndsWith.ToString(), true };

            yield return new object[] { "", "", StringCompareTypes.Null.ToString(), true };
            yield return new object[] { null, "", StringCompareTypes.Null.ToString(), true };
            yield return new object[] { "", "test", StringCompareTypes.Null.ToString(), true };
            yield return new object[] { "test", "test", StringCompareTypes.Null.ToString(), false };

            yield return new object[] { "", "", StringCompareTypes.NotNull.ToString(), false };
            yield return new object[] { null, "", StringCompareTypes.NotNull.ToString(), false };
            yield return new object[] { "", "test", StringCompareTypes.NotNull.ToString(), false };
            yield return new object[] { "test", "test", StringCompareTypes.NotNull.ToString(), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare(string baseObject, string compareObject, string compareType, bool result)
        {
            base.Should_Compare(baseObject, compareObject, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public override void Should_Compare_With_ObjectComparatorCondition(string baseObject, string compareObject, string compareType, bool result)
        {
            base.Should_Compare_With_ObjectComparatorCondition(baseObject, compareObject, compareType, result);
        }
    }
}
