using System.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching.Memory
{
    public class ThreadSafeObjectCache_Tests
    {
        [Fact]
        public void Test_ThreadSafeObjectCache()
        {
            var cache = new ThreadSafeObjectCache<string>(new MemoryCache("ThreadSafeObjectCache_Tests"));

            var cacheCallCount = 0;

            var aValue = cache.Get("A", () =>
            {
                ++cacheCallCount;
                return "A-value-1";
            });

            aValue.ShouldBe("A-value-1");
            cacheCallCount.ShouldBe(1);

            var bValue = cache.Get("B", () =>
            {
                ++cacheCallCount;
                return "B-value-1";
            });
            
            bValue.ShouldBe("B-value-1");
            cacheCallCount.ShouldBe(2);

            aValue = cache.Get("A", () =>
            {
                ++cacheCallCount;
                return "A-value-2";
            });
            
            aValue.ShouldBe("A-value-1");
            cacheCallCount.ShouldBe(2);
        }
    }
}
