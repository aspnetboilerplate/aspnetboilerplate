using System;
using Abp.Json;
using Abp.Timing;
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

        [Fact]
        public void Test_2()
        {
            Clock.Provider = ClockProviders.Utc;

            var str = "Abp.Tests.Json.JsonSerializationHelper_Tests+MyClass2, Abp.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null|{\"Date\":\"2016-04-13T16:58:10.526+08:00\"}";
            var result = (MyClass2)JsonSerializationHelper.DeserializeWithType(str);
            result.ShouldNotBeNull();
            result.Date.ShouldBe(new DateTime(2016, 04, 13, 08, 58, 10, 526, Clock.Kind));
            result.Date.Kind.ShouldBe(Clock.Kind);
        }

        [Fact]
        public void Test_3()
        {
            Clock.Provider = ClockProviders.Local;

            var myClass = new MyClass2(new DateTime(2016, 04, 13, 08, 58, 10, 526, Clock.Kind));
            var str = JsonSerializationHelper.SerializeWithType(myClass);
            var result = (MyClass2)JsonSerializationHelper.DeserializeWithType(str);

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