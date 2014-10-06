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
            DateTimeRange.ThisMonth.StartTime.ShouldBeGreaterThan(DateTimeRange.LastMonth.EndTime);
            DateTimeRange.ThisMonth.EndTime.ShouldBeLessThan(DateTimeRange.NextMonth.EndTime);

            DateTimeRange.LastMonth.StartTime.Day.ShouldBe(1);
            DateTimeRange.NextMonth.StartTime.Day.ShouldBe(1);
        }
    }
}