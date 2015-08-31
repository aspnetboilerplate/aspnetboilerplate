using System;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching.Memory
{
    public class MemoryCacheProvider_Tests : TestBaseWithLocalIocManager
    {
        private readonly ICacheProvider _cacheProvider;
        private ICacheStore<string, MyCacheItem> _cacheStore;

        public MemoryCacheProvider_Tests()
        {
            LocalIocManager.Register<ICacheProvider, MemoryCacheProvider>();
            _cacheProvider = LocalIocManager.Resolve<ICacheProvider>();
            _cacheStore = _cacheProvider.GetCacheStore<string, MyCacheItem>("MyCacheItems");
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cacheStore.GetOrDefault("A").ShouldBe(null);
            
            _cacheStore.Set("A", new MyCacheItem { Value = 42 });
            
            _cacheStore.GetOrDefault("A").ShouldNotBe(null);
            _cacheStore.GetOrDefault("A").Value.ShouldBe(42);
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}
