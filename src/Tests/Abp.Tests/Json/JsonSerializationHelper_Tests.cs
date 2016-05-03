using Abp.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests.Json
{
    public class JsonSerializationHelper_Tests
    {
        [Fact]
        public void Test_1()
        {
            var str = JsonSerializationHelper.SerializeWithType(new MyClass1("John"));
            var result = (MyClass1)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Name.ShouldBe("John");
        }

        public class MyClass1
        {
            public string Name { get; set; }

            public MyClass1()
            {
            }

            public MyClass1(string name)
            {
                Name = name;
            }
        }
    }
}