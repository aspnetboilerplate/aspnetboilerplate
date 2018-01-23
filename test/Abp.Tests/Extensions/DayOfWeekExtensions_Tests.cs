using System;
using Abp.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class DayOfWeekExtensions_Tests
    {
        [Fact]
        public void Weekend_Weekday_Test()
        {
            DayOfWeek.Monday.IsWeekday().ShouldBe(true);
            DayOfWeek.Monday.IsWeekend().ShouldBe(false);

            DayOfWeek.Saturday.IsWeekend().ShouldBe(true);
            DayOfWeek.Saturday.IsWeekday().ShouldBe(false);

            var datetime1 = new DateTime(2014, 10, 5, 16, 37, 25); //Sunday
            var datetime2 = new DateTime(2014, 10, 7, 16, 37, 25); //Tuesday

            datetime1.DayOfWeek.IsWeekend().ShouldBe(true);
            datetime2.DayOfWeek.IsWeekend().ShouldBe(false);

            datetime1.DayOfWeek.IsWeekday().ShouldBe(false);
            datetime2.DayOfWeek.IsWeekday().ShouldBe(true);
        }

        [Fact]
        public void FindNthWeekDayOfMonth_Test()
        {
            var firstMondayOfJan2018 = DayOfWeek.Monday.FindNthWeekDayOfMonth(2018, 1, 1);
            firstMondayOfJan2018.ShouldBe(new DateTime(2018, 1, 1));

            var secondFridayOfJan2018 = DayOfWeek.Friday.FindNthWeekDayOfMonth(2018, 1, 2);

            secondFridayOfJan2018.ShouldBe(new DateTime(2018, 1, 12));

            var lastSundayOfJan2018 = DayOfWeek.Sunday.FindNthWeekDayOfMonth(2018, 1, 5);

            lastSundayOfJan2018.ShouldBe(new DateTime(2018, 1, 28));

            var lastWednesdayOfJan2018 = DayOfWeek.Wednesday.FindNthWeekDayOfMonth(2018, 1, 5);

            lastWednesdayOfJan2018.ShouldBe(new DateTime(2018, 1, 31));
        }

        [Fact]
        public void TotalInstancesInMonth_Test()
        {
            var totalSundaysInJan2018 = DayOfWeek.Sunday.TotalInstancesInMonth(2018, 1);

            totalSundaysInJan2018.ShouldBe(4);

            var totalWednesdaysInJan2018 = DayOfWeek.Wednesday.TotalInstancesInMonth(2018, 1);

            totalWednesdaysInJan2018.ShouldBe(5);
        }
    }
}