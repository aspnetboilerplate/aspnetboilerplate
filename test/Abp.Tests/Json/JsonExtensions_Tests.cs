using System;
using Abp.Json;
using Abp.Timing;
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

        [Fact]
        public void ToJsonString_Not_Normalize_DateTime_Test()
        {
            Clock.Provider = ClockProviders.Utc;

            var model1 = new MyClass1
            {
                Date = new DateTime(2016, 03, 16, 14, 0, 0, DateTimeKind.Local)
            };

            var model1AsString = model1.ToJsonString();
            var date1 = model1AsString.Replace("{\"Date\":", "").Replace("\"}", "").Replace("\"", "");
            date1.ShouldNotEndWith("00Z");

            var model2 = new MyClass2
            {
                Date = new DateTime(2016, 03, 16, 14, 0, 0, DateTimeKind.Local)
            };

            var model2AsString = model2.ToJsonString();
            var date2 = model2AsString.Replace("{\"Date\":", "").Replace("\"}", "").Replace("\"", "");
            date2.ShouldNotEndWith("00Z");
        }

        [DisableDateTimeNormalization]
        public class MyClass1
        {
            public DateTime Date { get; set; }
        }

        public class MyClass2
        {
            [DisableDateTimeNormalization]
            public DateTime Date { get; set; }
        }
    }
}
