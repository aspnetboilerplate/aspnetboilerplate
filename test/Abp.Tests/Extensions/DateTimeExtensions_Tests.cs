using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class DateTimeExtensions_Tests
    {
        [Fact]
        public void ToUnixTimestamp_Test()
        {
            var timestamp = new DateTime(1980, 11, 20).ToUnixTimestamp();
            timestamp.ShouldBe(343526400);
        }

        [Fact]
        public void FromUnixTimestamp_Test()
        {
            var date = 343526400d.FromUnixTimestamp();
            date.ShouldBe(new DateTime(1980, 11, 20));
        }

        [Fact]
        public void ToDayEnd_Test()
        {
            var now = Clock.Now;

            var dateEnd = now.ToDayEnd();

            dateEnd.ShouldBe(now.Date.AddDays(1).AddMilliseconds(-1));
        }

        [Fact]
        public void StartOfWeek_Test()
        {
            var startOfWeekSunday = new DateTime(1980, 11, 20).StartOfWeek(DayOfWeek.Sunday);

            startOfWeekSunday.ShouldBe(new DateTime(1980, 11, 16));

            var startOfWeekMonday = new DateTime(1980, 11, 20).StartOfWeek(DayOfWeek.Monday);

            startOfWeekMonday.ShouldBe(new DateTime(1980, 11, 17));
        }

        [Fact]
        public void DaysOfMonth_Test()
        {
            var days = DateTimeExtensions.DaysOfMonth(2018, 1);

            days.ShouldNotBeNull();

            days.Count().ShouldBe(31);

        }

        [Fact]
        public void WeekDayInstanceOfMonth_Test()
        {
            var instance = new DateTime(2011, 11, 29).WeekDayInstanceOfMonth();

            instance.ShouldBe(5);
        }

        [Fact]
        public void TotalDaysInMonth_Test()
        {
            var totalDays = new DateTime(2018, 1, 15).TotalDaysInMonth();
            totalDays.ShouldBe(31);
        }

        [Fact]
        public void ToDateTimeUnspecified_Test()
        {
            var localTime = Clock.Now;

            var unspecified = localTime.ToDateTimeUnspecified();

            unspecified.Kind.ShouldBe(DateTimeKind.Unspecified);

        }

        [Fact]
        public void TrimMilliseconds_Test()
        {
            var now = Clock.Now;

            var trimmed = now.TrimMilliseconds();

            trimmed.Millisecond.ShouldBe(0);
        }
    }
}
