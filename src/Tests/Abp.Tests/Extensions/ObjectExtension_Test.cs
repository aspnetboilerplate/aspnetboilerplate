using Abp.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Extensions
{
    public class ObjectExtensions_Tests
    {
        [Fact]
        public void As_Test()
        {
            var obj = (object)new ObjectExtensions_Tests();
            obj.As<ObjectExtensions_Tests>().ShouldNotBe(null);

            obj = null;
            obj.As<ObjectExtensions_Tests>().ShouldBe(null);
        }

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
