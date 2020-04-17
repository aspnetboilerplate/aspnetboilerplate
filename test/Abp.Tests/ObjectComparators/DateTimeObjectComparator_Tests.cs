using System;
using System.Collections.Generic;
using Abp.ObjectComparators.DateTimeComparators;
using Xunit;

namespace Abp.Tests.ObjectComparators
{
    public class DateTimeObjectComparator_Tests : ObjectComparatorTestBase<DateTime, DateTimeCompareTypes>
    {
        public static IEnumerable<object[]> Should_Compare_Data_Generator()
        {
            yield return new object[] { "1996-03-18", "1996-03-19", nameof(DateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(DateTimeCompareTypes.Equals), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(DateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.Equals), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(DateTimeCompareTypes.Equals), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.Equals), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(DateTimeCompareTypes.LessThan), true };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(DateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(DateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.LessThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(DateTimeCompareTypes.LessThan), true };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.LessThan), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(DateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(DateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(DateTimeCompareTypes.LessOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(DateTimeCompareTypes.LessOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.LessOrEqualThan), false };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(DateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(DateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(DateTimeCompareTypes.BiggerThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(DateTimeCompareTypes.BiggerThan), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.BiggerThan), true };

            yield return new object[] { "1996-03-18", "1996-03-19", nameof(DateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "1996-03-19", "1996-03-19", nameof(DateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-20", "1996-03-19", nameof(DateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.BiggerOrEqualThan), true };
            yield return new object[] { "1996-03-19 18:00:00", "1996-03-19 18:00:01", nameof(DateTimeCompareTypes.BiggerOrEqualThan), false };
            yield return new object[] { "1996-03-19 18:00:01", "1996-03-19 18:00:00", nameof(DateTimeCompareTypes.BiggerOrEqualThan), true };
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public void Should_Compare_Datetime(string date1, string date2, string compareType, bool result)
        {
            DateTime dateTime1 = DateTime.Parse(date1);
            DateTime dateTime2 = DateTime.Parse(date2);

            base.Should_Compare(dateTime1, dateTime2, compareType, result);
        }

        [Theory]
        [MemberData(nameof(Should_Compare_Data_Generator))]
        public void Should_Compare_With_ObjectComparatorCondition_Datetime(string date1, string date2, string compareType, bool result)
        {
            DateTime dateTime1 = DateTime.Parse(date1);
            DateTime dateTime2 = DateTime.Parse(date2);

            base.Should_Compare_With_ObjectComparatorCondition(dateTime1, dateTime2, compareType, result);
        }
    }
}
