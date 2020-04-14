using System;
using System.Collections.Generic;
using Abp.Extensions;
using Abp.ObjectComparators.DateTimeComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class NullableDateTimeObjectComparator_Tests : ObjectComparatorTestBase<DateTime?, NullableDateTimeCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { "1996-03-18", "1996-03-19", nameof(NullableDateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(NullableDateTimeCompareTypes.Equals), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(NullableDateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.Equals), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(NullableDateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.Equals), false };
            yield return new object[] { "", "", nameof(NullableDateTimeCompareTypes.Equals), true };
            yield return new object[] { "", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19 18:00:00", "", nameof(NullableDateTimeCompareTypes.Equals), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessThan), true };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(NullableDateTimeCompareTypes.LessThan), true };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "", "", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "", nameof(NullableDateTimeCompareTypes.LessThan), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), false };
            yield return new object[] { "", "", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), false };
            yield return new object[] { "", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "", nameof(NullableDateTimeCompareTypes.LessOrEqualThan), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerThan), true };
            yield return new object[] { "", "", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "", nameof(NullableDateTimeCompareTypes.BiggerThan), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "", "", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "", "1996-03-19 18:00:00", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "", nameof(NullableDateTimeCompareTypes.BiggerOrEqualThan), false };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public void Should_Compare_Datetime(string date1, string date2, string compareType, bool result)
        {
            DateTime? dateTime1 = date1.IsNullOrWhiteSpace() ? (DateTime?)null : DateTime.Parse(date1);
            DateTime? dateTime2 = date2.IsNullOrWhiteSpace() ? (DateTime?)null : DateTime.Parse(date2);

            base.Should_Compare(dateTime1, dateTime2, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public void Should_Compare_With_ObjectComparatorCondition_Datetime(string date1, string date2, string compareType, bool result)
        {
            DateTime? dateTime1 = date1.IsNullOrWhiteSpace() ? (DateTime?)null : DateTime.Parse(date1);
            DateTime? dateTime2 = date2.IsNullOrWhiteSpace() ? (DateTime?)null : DateTime.Parse(date2);

            base.Should_Compare_With_ObjectComparatorCondition(dateTime1, dateTime2, compareType, result);
        }
    }
}
