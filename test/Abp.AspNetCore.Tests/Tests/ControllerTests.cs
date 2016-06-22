using System.Threading.Tasks;
using Abp.AspNetCore.App.Models;
using Abp.Web.Mvc.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class ControllerTests : AbpAspNetCoreTestBase
    {
        [Fact]
        public async Task Should_Return_SimpleContent()
        {
            // Act
            var response = await GetResponseAsStringAsync("/Test/SimpleContent");

            // Assert
            response.ShouldBe("Hello world...");
        }

        [Fact]
        public async Task Should_Wrap_SimpleJson_By_Default()
        {
            // Act
            var ajaxResponse = await GetResponseAsObjectAsync<MvcAjaxResponse<SimpleViewModel>>("/Test/SimpleJson");

            //Assert
            ajaxResponse.Result.StrValue.ShouldBe("Forty Two");
            ajaxResponse.Result.IntValue.ShouldBe(42);
        }
    }
}