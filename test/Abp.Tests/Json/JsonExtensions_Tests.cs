using Abp.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests.Json
{
    public class JsonExtensions_Tests
    {
        [Fact]
        public void ToJsonString_Test()
        {
            42.ToJsonString().ShouldBe("42");
        }
    }
}
