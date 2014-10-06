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
        }
    }
}