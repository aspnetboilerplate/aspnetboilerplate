using Abp.Runtime.Caching.Memory;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching.Memory
{
    public class AbpMemoryCache_Tests : TestBaseWithLocalIocManager
    {
        private readonly AbpMemoryCache _memoryCache;

        public AbpMemoryCache_Tests()
        {
            _memoryCache = new AbpMemoryCache("test cache");
        }


        [Fact]
        public void Single_Key_Get_Test()
        {
            var cacheValue = _memoryCache.GetOrDefault("A");
            cacheValue.ShouldBeNull();

            cacheValue = _memoryCache.Get("A", (key) => "test");
            cacheValue.ShouldBe("test");
        }

        [Fact]
        public void Multi_Keys_Get_Test()
        {
            var cacheValues = _memoryCache.GetOrDefault(new[] { "A", "B" });
            cacheValues.ShouldNotBeNull();
            cacheValues.Length.ShouldBe(2);
            cacheValues[0].ShouldBeNull();
            cacheValues[1].ShouldBeNull();

            cacheValues = _memoryCache.Get(new[] { "A", "B" }, (key) => "test " + key);
            cacheValues.ShouldNotBeNull();
            cacheValues.Length.ShouldBe(2);
            cacheValues[0].ShouldBe("test A");
            cacheValues[1].ShouldBe("test B");
        }
    }
}
