using Abp.Configuration;
using NUnit.Framework;

namespace Abp.Tests.Configuration
{
    [TestFixture]
    public class DictionaryBasedConfig_Test
    {
        private MyConfig _config;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _config = new MyConfig();
        }

        [Test]
        public void Should_Get_Value()
        {
            var testObject = new TestClass {Value = 42};

            _config["IntValue"] = 42;
            _config["StringValue"] = "Test string";
            _config["ObjectValue"] = testObject;

            Assert.AreEqual(42, _config["IntValue"]);
            Assert.AreEqual(42, _config.GetOrDefault<int>("IntValue"));

            Assert.AreEqual("Test string", _config["StringValue"]);
            Assert.AreEqual("Test string", _config.GetOrDefault<string>("StringValue"));

            Assert.AreEqual(testObject, _config["ObjectValue"]);
            Assert.AreEqual(testObject, _config.GetOrDefault<TestClass>("ObjectValue"));
            Assert.AreEqual(42, _config.GetOrDefault<TestClass>("ObjectValue").Value);
        }

        [Test]
        public void Should_Get_Default_If_No_Value()
        {
            Assert.AreEqual(null, _config["MyUndefinedName"]);
            Assert.AreEqual(null, _config.GetOrDefault<string>("MyUndefinedName"));
            Assert.AreEqual(null, _config.GetOrDefault<MyConfig>("MyUndefinedName"));
            Assert.AreEqual(0, _config.GetOrDefault<int>("MyUndefinedName"));
        }

        private class MyConfig : DictionayBasedConfig
        {

        }

        private class TestClass
        {
            public int Value { get; set; }
        }
    }
}
