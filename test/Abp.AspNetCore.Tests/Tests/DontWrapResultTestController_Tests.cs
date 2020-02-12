using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class DontWrapResultTestController_Tests : AppTestBase
    {
        [Fact]
        public async Task DontWrapResultTestControllerTests_Get_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<DontWrapResultTestController>(
                    nameof(DontWrapResultTestController.Get)
                )
            );

            // Assert
            response.ShouldBe("42");
        }

        [Fact]
        public async Task DontWrapResultTestControllerTests_GetBase_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<DontWrapResultTestController>(
                    nameof(DontWrapResultTestController.GetBase)
                )
            );

            // Assert
            response.ShouldBe("42");
        }
    }
}