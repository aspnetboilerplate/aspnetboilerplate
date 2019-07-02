using System;
using System.Collections.Generic;
using Abp.Json;
using Abp.Runtime.Caching;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching
{
    public class AbpCacheData_Tests
    {
        [Fact]
        public void Serialize_List_Test()
        {
            List<string> source = new List<string>
            {
                "Stranger Things",
                "The OA",
                "Lost in Space"
            };

            var result = AbpCacheData.Serialize(source);
            result.Type.ShouldStartWith("System.Collections.Generic.List`1[[System.String,");
            result.Payload.ShouldBe("[\"Stranger Things\",\"The OA\",\"Lost in Space\"]");
        }

        [Fact]
        public void Serialize_Class_Test()
        {
            var source = new MyTestClass
            {
                Field1 = 42,
                Field2 = "Stranger Things"
            };

            var result = AbpCacheData.Serialize(source);
            result.Type.ShouldBe("Abp.Tests.Runtime.Caching.AbpCacheData_Tests+MyTestClass, Abp.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            result.Payload.ShouldBe("{\"Field1\":42,\"Field2\":\"Stranger Things\"}");
        }

        [Fact]
        public void Deserialize_List_Test()
        {
            var json = "{\"Payload\":\"[\\\"Stranger Things\\\",\\\"The OA\\\",\\\"Lost in Space\\\"]\",\"Type\":\"System.Collections.Generic.List`1[[System.String]]\"}";
            var cacheData = AbpCacheData.Deserialize(json);

            cacheData.ShouldNotBeNull();
        }

        [Fact]
        public void Deserialize_Class_Test()
        {
            var json = "{\"Payload\": \"{\\\"Field1\\\": 42,\\\"Field2\\\":\\\"Stranger Things\\\"}\",\"Type\":\"Abp.Tests.Runtime.Caching.AbpCacheData_Tests+MyTestClass, Abp.Tests\"}";

            var cacheData = AbpCacheData.Deserialize(json);

            cacheData.ShouldNotBeNull();
        }

        class MyTestClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
