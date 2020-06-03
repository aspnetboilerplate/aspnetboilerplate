using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.App.AppServices;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class ResponseNoCacheTestAppService_Tests: AppTestBase
    {
        [Fact]
        public async Task ResponseNoCacheAppService_Get_Test()
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<ResponseNoCacheTestAppService>(
                    nameof(ResponseNoCacheTestAppService.Get)
                )
            );

            // Assert
            response.Headers.CacheControl.ShouldNotBeNull();
            response.Headers.CacheControl.NoCache.ShouldBeTrue();
            response.Headers.CacheControl.NoStore.ShouldBeTrue();
            response.Headers.Pragma.Single().Name.ShouldBe("no-cache");
        }

        [Fact]
        public async Task ResponseNoCacheAppService_Get_With_Cache_Test()
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<ResponseNoCacheTestAppService>(
                    nameof(ResponseNoCacheTestAppService.GetWithCache)
                )
            );

            // Assert
            response.Headers.CacheControl.ShouldNotBeNull();
            response.Headers.CacheControl.MaxAge.ShouldBe(TimeSpan.FromSeconds(20));
            response.Headers.CacheControl.Private.ShouldBeTrue();
            response.Headers.Pragma.Count.ShouldBe(0);
        }
    }
}