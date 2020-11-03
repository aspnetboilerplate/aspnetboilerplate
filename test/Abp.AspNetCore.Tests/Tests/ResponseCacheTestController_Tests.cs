using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class ResponseCacheTestController_Tests : AppTestBase
    {
        [Fact]
        public async Task ResponseCacheController_Get_Test()
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<ResponseCacheTestController>(
                    nameof(ResponseCacheTestController.Get)
                )
            );

            // Assert
            response.Headers.CacheControl.ShouldBeNull();
            response.Headers.Pragma.Count.ShouldBe(0);
        }

        [Fact]
        public async Task ResponseCacheController_Get_With_Cache_Test()
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<ResponseCacheTestController>(
                    nameof(ResponseCacheTestController.GetWithCache)
                )
            );

            // Assert
            response.Headers.CacheControl.ShouldNotBeNull();
            response.Headers.CacheControl.MaxAge.ShouldBe(TimeSpan.FromSeconds(60));
            response.Headers.CacheControl.Public.ShouldBeTrue();
            response.Headers.Pragma.Count.ShouldBe(0);
        }

        [Fact]
        public async Task ResponseCacheController_Get_Without_Cache_Test()
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<ResponseCacheTestController>(
                    nameof(ResponseCacheTestController.GetWithoutCache)
                )
            );

            // Assert
            response.Headers.CacheControl.ShouldNotBeNull();
            response.Headers.CacheControl.NoCache.ShouldBeTrue();
            response.Headers.CacheControl.NoStore.ShouldBeTrue();
            response.Headers.Pragma.Single().Name.ShouldBe("no-cache");
        }
    }
}