using Abp.Configuration;
using NUnit.Framework;
using Shouldly;
using Xunit;

namespace Abp.Tests.Configuration
{
    public class DictionaryBasedConfig_Test
    {
        private readonly MyConfig _config;

        public DictionaryBasedConfig_Test()
        {
            _config = new MyConfig();
        }

        [Fact]
        public void Should_Get_Value()
        {
            var testObject = new TestClass {Value = 42};

            _config["IntValue"] = 42;
            _config["StringValue"] = "Test string";
            _config["ObjectValue"] = testObject;

            _config["IntValue"].ShouldBe(42);
            _config.Get<int>("IntValue").ShouldBe(42);

            _config["StringValue"].ShouldBe("Test string");
            _config.Get<string>("StringValue").ShouldBe("Test string");

            _config["ObjectValue"].ShouldBeSameAs(testObject);
            _config.Get<TestClass>("ObjectValue").ShouldBeSameAs(testObject);
            _config.Get<TestClass>("ObjectValue").Value.ShouldBe(42);
        }

        [Fact]
        public void Should_Get_Default_If_No_Value()
        {
            _config["MyUndefinedName"].ShouldBe(null);
            _config.Get<string>("MyUndefinedName").ShouldBe(null);
            _config.Get<MyConfig>("MyUndefinedName").ShouldBe(null);
            _config.Get<int>("MyUndefinedName").ShouldBe(0);
        }

        private class MyConfig : DictionaryBasedConfig
        {

        }

        private class TestClass
        {
            public int Value { get; set; }
        }
    }
}
