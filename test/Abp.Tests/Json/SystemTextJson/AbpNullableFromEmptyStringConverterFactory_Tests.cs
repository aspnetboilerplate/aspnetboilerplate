using System;
using System.Text.Json;
using Abp.Json.SystemTextJson;
using Shouldly;
using Xunit;

namespace Abp.Tests.Json.SystemTextJson
{
    public class AbpNullableFromEmptyStringConverterFactory_Tests
    {
        [Fact]
        public void Test_Read()
        {
            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new AbpNullableFromEmptyStringConverterFactory()
                }
            };

            var testClass = JsonSerializer.Deserialize<TestClass>("{\"Prop1\": \"\", \"Prop2\": \"\", \"Prop3\": \"\", \"Prop4\": \"\", \"Prop5\": \"\"}", options);
            testClass.ShouldNotBeNull();
            testClass.Prop1.ShouldBeNull();
            testClass.Prop2.ShouldBeNull();
            testClass.Prop3.ShouldBeNull();
            testClass.Prop4.ShouldBeNull();
            testClass.Prop5.ShouldBe("");
        }

        [Fact]
        public void Test_Write()
        {
            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new AbpStringToBooleanConverter()
                }
            };

            var testClassJson = JsonSerializer.Serialize(new TestClass()
            {
                Prop1 = null,
                Prop2 = null,
                Prop3 = null,
                Prop4 = null,
                Prop5 = ""
            }, options);

            testClassJson.ShouldBe("{\"Prop1\":null,\"Prop2\":null,\"Prop3\":null,\"Prop4\":null,\"Prop5\":\"\"}");
        }

        class TestClass
        {
            public int? Prop1 { get; set; }

            public DateTime? Prop2 { get; set; }

            public double? Prop3 { get; set; }

            public TimeSpan? Prop4 { get; set; }

            public string Prop5 { get; set; }
        }
    }
}
