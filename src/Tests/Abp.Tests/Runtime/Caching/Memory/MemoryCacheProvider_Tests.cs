using System;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching.Memory
{
    public class MemoryCacheProvider_Tests : TestBaseWithLocalIocManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<string, MyCacheItem> _cache;

        public MemoryCacheProvider_Tests()
        {
            LocalIocManager.Register<ICacheManager, AbpMemoryCacheManager>();
            _cacheManager = LocalIocManager.Resolve<ICacheManager>();
            _cache = _cacheManager.GetCache("MyCacheItems").AsTyped<string, MyCacheItem>();
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.GetOrDefault("A").ShouldBe(null);
            
            _cache.Set("A", new MyCacheItem { Value = 42 });
            
            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}
