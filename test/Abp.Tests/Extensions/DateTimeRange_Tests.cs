using System;
using System.Linq;
using Abp.Extensions;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class DateTimeRange_Tests
    {
        [Fact]
        public void StaticRanges_Test()
        {
            DateTimeRange.Today.StartTime.ShouldBeGreaterThan(DateTimeRange.Yesterday.EndTime);
            DateTimeRange.Today.EndTime.ShouldBeLessThan(DateTimeRange.Tomorrow.StartTime);

            DateTimeRange.ThisMonth.StartTime.Day.ShouldBe(1);
            DateTimeRange.LastMonth.StartTime.Day.ShouldBe(1);
            DateTimeRange.NextMonth.StartTime.Day.ShouldBe(1);

            DateTimeRange.ThisMonth.StartTime.ShouldBeGreaterThan(DateTimeRange.LastMonth.EndTime);
            DateTimeRange.ThisMonth.EndTime.ShouldBeLessThan(DateTimeRange.NextMonth.EndTime);

            DateTimeRange.ThisYear.StartTime.Month.ShouldBe(1);
            DateTimeRange.ThisYear.StartTime.Day.ShouldBe(1);
            
            DateTimeRange.ThisYear.StartTime.ShouldBeGreaterThan(DateTimeRange.LastYear.EndTime);
            DateTimeRange.ThisYear.EndTime.ShouldBeLessThan(DateTimeRange.NextYear.StartTime);

            DateTimeRange.Last7DaysExceptToday.EndTime.ShouldBeLessThan(DateTimeRange.Today.StartTime);
            DateTimeRange.Last30DaysExceptToday.EndTime.ShouldBeLessThan(DateTimeRange.Today.StartTime);
        }

        [Fact]
        public void DaysInRange_Test()
        {
            var now = Clock.Now;
            var dateTimeRange = new DateTimeRange(now.Date, now.Date.AddDays(1));

            var days = dateTimeRange.DaysInRange();

            days.ShouldNotBeNull();
            days.Count().ShouldBe(1);
            days.Single().ShouldBe(now.Date);

            var year = new DateTime(2018,1,1);
            var yearRange = new DateTimeRange(year, year.AddYears(1));

            var yearDays = yearRange.DaysInRange();
            yearDays.ShouldNotBeNull();
            yearDays.Count().ShouldBe(365);
            yearDays.FirstOrDefault().ShouldBe(year);
        }
    }
}