using Abp.Data;
using Abp.Runtime.Caching;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Abp.Tests.Runtime.Caching
{
    public class AbpCacheBase_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Single_Key_Get_Test()
        {
            var cache1 = new MyCache1("cache 1");
            var cacheValue1 = cache1.GetOrDefault("A");
            cacheValue1.ShouldBe(0);

            cacheValue1 = cache1.Get("A", (key) => 1);
            cacheValue1.ShouldBe(1);

            var cache2 = new MyCache2("cache 2");
            var cacheValue2 = cache2.GetOrDefault("B");
            cacheValue2.ShouldBeNull();

            cacheValue2 = cache2.Get("B", (key) => 2);
            cacheValue2.ShouldBe(2);
        }

        [Fact]
        public void Multi_Keys_Get_Test()
        {
            var cache1 = new MyCache1("cache 1");
            var cacheValues1 = cache1.GetOrDefault(new[] { "A", "B" });
            cacheValues1.ShouldNotBeNull();
            cacheValues1.Length.ShouldBe(2);
            cacheValues1[0].ShouldBe(0);
            cacheValues1[1].ShouldBe(0);

            cacheValues1 = cache1.Get(new[] { "A", "B" }, (key) => 11);
            cacheValues1.ShouldNotBeNull();
            cacheValues1.Length.ShouldBe(2);
            cacheValues1[0].ShouldBe(11);
            cacheValues1[1].ShouldBe(11);

            var cache2 = new MyCache2("cache 2");
            var cacheValues2 = cache2.GetOrDefault(new[] { "C", "D" });
            cacheValues2.ShouldNotBeNull();
            cacheValues2.Length.ShouldBe(2);
            cacheValues2[0].ShouldBeNull();
            cacheValues2[1].ShouldBeNull();

            cacheValues2 = cache2.Get(new[] { "C", "D" }, (key) => 22);
            cacheValues2.ShouldNotBeNull();
            cacheValues2.Length.ShouldBe(2);
            cacheValues2[0].ShouldBe(22);
            cacheValues2[1].ShouldBe(22);
        }

        class MyCache1 : AbpCacheBase<string, int>
        {
            private readonly Dictionary<string, int> _collection;

            public MyCache1(string name) : base(name)
            {
                _collection = new Dictionary<string, int>();
            }

            public override void Clear()
            {
                _collection.Clear();
            }

            public override bool TryGetValue(string key, out int value)
            {
                return _collection.TryGetValue(key, out value);
            }

            public override void Remove(string key)
            {
                _collection.Remove(key);
            }

            public override void Set(string key, int value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
            {
                _collection.Add(key, value);
            }
        }

        class MyCache2 : AbpCacheBase<string, int?>
        {
            private readonly Dictionary<string, int?> _collection;
            
            public MyCache2(string name) : base(name)
            {
                _collection = new Dictionary<string, int?>();
            }

            public override void Clear()
            {
                _collection.Clear();
            }

            public override bool TryGetValue(string key, out int? value)
            {
                return _collection.TryGetValue(key, out value);
            }

            public override void Remove(string key)
            {
                _collection.Remove(key);
            }

            public override void Set(string key, int? value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
            {
                _collection.Add(key, value);
            }
        }
    }
}
