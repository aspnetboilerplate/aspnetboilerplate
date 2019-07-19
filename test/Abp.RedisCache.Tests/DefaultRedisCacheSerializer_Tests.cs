using System.Collections.Generic;
using System.Linq;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Redis;
using Abp.Tests;
using Abp.Tests.Runtime.Caching;
using Shouldly;
using Xunit;

namespace Abp.RedisCache.Tests
{
    public class DefaultRedisCacheSerializer_Tests : TestBaseWithLocalIocManager
    {
        private IRedisCacheSerializer _redisCacheSerializer;

        public DefaultRedisCacheSerializer_Tests()
        {
            LocalIocManager.Register<IRedisCacheSerializer, DefaultRedisCacheSerializer>();
            _redisCacheSerializer = LocalIocManager.Resolve<IRedisCacheSerializer>();
        }

        [Fact]
        public void Serialize_List_Test()
        {
            List<string> source = new List<string>
            {
                "Stranger Things",
                "The OA",
                "Lost in Space"
            };

            var result = _redisCacheSerializer.Serialize(source);
            result.ShouldBe(source.ToJsonString());
        }

        [Fact]
        public void Serialize_Class_Test()
        {
            var source = new MyTestClass
            {
                Field1 = 42,
                Field2 = "Stranger Things"
            };

            var result = _redisCacheSerializer.Serialize(source);
            result.ShouldBe(source.ToJsonString());
        }

        [Fact]
        public void Deserialize_List_Test()
        {
            var json = "[\"Stranger Things\",\"The OA\",\"Lost in Space\"]";
            var list = _redisCacheSerializer.Deserialize<List<string>>(json);

            list.ShouldNotBeNull();
            list.Count.ShouldBe(3);
            list.First().ShouldBe("Stranger Things");
        }

        [Fact]
        public void Deserialize_Class_Test()
        {
            var json = "{\"Field1\":42,\"Field2\":\"Stranger Things\"}";
            var myTestClass = _redisCacheSerializer.Deserialize<MyTestClass>(json);

            myTestClass.ShouldNotBeNull();
            myTestClass.Field1.ShouldBe(42);
            myTestClass.Field2.ShouldBe("Stranger Things");
        }

        class MyTestClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
