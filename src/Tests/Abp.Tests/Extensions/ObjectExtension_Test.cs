using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class ObjectExtension_Test
    {
        [Fact]
        public void IsIn_Test()
        {
            5.IsIn(1, 3, 5, 7).ShouldBe(true);
            6.IsIn(1, 3, 5, 7).ShouldBe(false);

            int? number = null;
            number.IsIn(2, 3, 5).ShouldBe(false);

            var str = "a";
            str.IsIn("a", "b", "c").ShouldBe(true);

            str = null;
            str.IsIn("a", "b", "c").ShouldBe(false);
        }
    }
}
