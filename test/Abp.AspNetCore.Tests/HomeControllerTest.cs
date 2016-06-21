using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class HomeControllerTest : AbpAspNetCoreTestBase
    {
        [Fact]
        public async Task Should_Simply_Return_Content_Result()
        {
            // Act
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldBe("Hello world...");
        }
    }
}