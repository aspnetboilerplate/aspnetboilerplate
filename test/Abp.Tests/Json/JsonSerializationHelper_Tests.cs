using System;
using Abp.Json;
using Abp.Localization;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.Tests.Json
{
    public class JsonSerializationHelper_Tests
    {
        [Fact]
        public void Should_Simply_Serialize_And_Deserialize()
        {
            var str = JsonSerializationHelper.SerializeWithType(new LocalizableString("Foo", "Bar"));
            var result = (LocalizableString)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Foo");
            result.SourceName.ShouldBe("Bar");
        }

        [Fact]
        public void Should_Deserialize_With_Different_Assembly_Version()
        {
            var str = "Abp.Localization.LocalizableString, Abp, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null|{\"SourceName\":\"Bar\",\"Name\":\"Foo\"}";
            var result = (LocalizableString)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Foo");
            result.SourceName.ShouldBe("Bar");
        }

        [Fact]
        public void Should_Deserialize_With_DateTime()
        {
            Clock.Provider = ClockProviders.Utc;

            var str = "Abp.Tests.Json.JsonSerializationHelper_Tests+MyClass2, Abp.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null|{\"Date\":\"2016-04-13T16:58:10.526+08:00\"}";
            var result = (MyClass2)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Date.ShouldBe(new DateTime(2016, 04, 13, 08, 58, 10, 526, Clock.Kind));
            result.Date.Kind.ShouldBe(Clock.Kind);
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

        public class MyClass2
        {
            public DateTime Date { get; set; }

            public MyClass2(DateTime date)
            {
                Date = date;
            }
        }
    }
}