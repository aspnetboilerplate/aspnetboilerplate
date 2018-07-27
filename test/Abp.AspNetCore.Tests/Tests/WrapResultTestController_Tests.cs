using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class WrapResultTestController_Tests: AppTestBase
    {
        [Fact]
        public async Task WrapResultTestControllerTests_Wrap_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<WrapResultTestController>(
                    nameof(WrapResultTestController.Get)
                )
            );

            // Assert
            response.ShouldNotBe("42");
        }

        [Fact]
        public async Task WrapResultTestControllerTests_DontWrap_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<WrapResultTestController>(
                    nameof(WrapResultTestController.GetDontWrap)
                )
            );

            // Assert
            response.ShouldBe("42");
        }
    }
}