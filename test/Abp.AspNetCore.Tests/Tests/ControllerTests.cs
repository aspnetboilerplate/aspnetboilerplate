using System.Net;
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
        public async Task Should_Return_Content()
        {
            // Act
            var response = await GetResponseAsStringAsync("/Test/SimpleContent");

            // Assert
            response.ShouldBe("Hello world...");
        }

        [Fact]
        public async Task Should_Wrap_Json_By_Default()
        {
            // Act
            var response = await GetResponseAsObjectAsync<MvcAjaxResponse<SimpleViewModel>>("/Test/SimpleJson");

            //Assert
            response.Result.StrValue.ShouldBe("Forty Two");
            response.Result.IntValue.ShouldBe(42);
        }

        [Theory]
        [InlineData(true, "This is a user friendly exception message")]
        [InlineData(false, "This is an exception message")]
        public async Task Should_Wrap_Json_Exception_By_Default(bool userFriendly, string message)
        {
            // Act
            var response = await GetResponseAsObjectAsync<MvcAjaxResponse<SimpleViewModel>>(
                               "/Test/SimpleJsonException" + "?message=" + message + "&userFriendly=" + userFriendly,
                               HttpStatusCode.InternalServerError
                           );

            //Assert
            response.Error.ShouldNotBeNull();
            if (userFriendly)
            {
                response.Error.Message.ShouldBe(message);
            }
            else
            {
                response.Error.Message.ShouldNotBe(message);
            }
        }

        [Fact]
        public async Task Should_Not_Wrap_Json_If_DontWrap_Declared()
        {
            // Act
            var response = await GetResponseAsObjectAsync<SimpleViewModel>("/Test/SimpleJsonDontWrap");

            //Assert
            response.StrValue.ShouldBe("Forty Two");
            response.IntValue.ShouldBe(42);
        }
    }
}