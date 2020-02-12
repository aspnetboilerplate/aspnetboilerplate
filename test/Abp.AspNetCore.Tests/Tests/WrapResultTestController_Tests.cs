using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        [Fact]
        public async Task WrapResultTestControllerTests_Xml_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<WrapResultTestController>(
                    nameof(WrapResultTestController.GetXml)
                )
            );

            // Assert
            var result = XElement.Parse(response).Elements().FirstOrDefault(x =>
                string.Equals(x.Name.ToString(), "result", StringComparison.InvariantCultureIgnoreCase));
            result.ShouldNotBeNull();
            result.Value.ShouldBe("42"); 
        }
    }
}