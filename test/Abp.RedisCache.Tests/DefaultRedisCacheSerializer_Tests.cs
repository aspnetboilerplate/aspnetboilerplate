using System.Collections.Generic;
using System.Linq;
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

            var result = _redisCacheSerializer.Serialize(source, typeof(List<string>));
            result.ShouldStartWith("{\"Payload\":\"[\\\"Stranger Things\\\",\\\"The OA\\\",\\\"Lost in Space\\\"]\",\"Type\":\"System.Collections.Generic.List`1[[System.String,");
        }

        [Fact]
        public void Serialize_Class_Test()
        {
            var source = new MyTestClass
            {
                Field1 = 42,
                Field2 = "Stranger Things"
            };

            var result = _redisCacheSerializer.Serialize(source, typeof(MyTestClass));
            result.ShouldBe("{\"Payload\":\"{\\\"Field1\\\":42,\\\"Field2\\\":\\\"Stranger Things\\\"}\",\"Type\":\"Abp.RedisCache.Tests.DefaultRedisCacheSerializer_Tests+MyTestClass, Abp.RedisCache.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"}");
        }

        [Fact]
        public void Deserialize_List_Test()
        {
            var json = "{\"Payload\":\"[\\\"Stranger Things\\\",\\\"The OA\\\",\\\"Lost in Space\\\"]\",\"Type\":\"System.Collections.Generic.List`1[[System.String]]\"}";
            var cacheData = _redisCacheSerializer.Deserialize(json);

            var typedCacheData = cacheData as List<string>;
            typedCacheData.ShouldNotBeNull();
            typedCacheData.Count.ShouldBe(3);
            typedCacheData.First().ShouldBe("Stranger Things");
        }

        [Fact]
        public void Deserialize_Class_Test()
        {
            var json = "{\"Payload\": \"{\\\"Field1\\\": 42,\\\"Field2\\\":\\\"Stranger Things\\\"}\",\"Type\":\"Abp.RedisCache.Tests.DefaultRedisCacheSerializer_Tests+MyTestClass, Abp.RedisCache.Tests\"}";

            var cacheData = _redisCacheSerializer.Deserialize(json);

            var typedCacheData = cacheData as MyTestClass;
            typedCacheData.ShouldNotBeNull();
            typedCacheData.Field1.ShouldBe(42);
            typedCacheData.Field2.ShouldBe("Stranger Things");
        }

        class MyTestClass
        {
            public int Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
