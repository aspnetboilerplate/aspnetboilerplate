using System.Threading.Tasks;
using Abp.AspNetCore.Tests.App.Models;
using Abp.Web.Mvc.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            var response = await GetSuccessResponseBodyAsString("/Test/SimpleContent");

            // Assert
            response.ShouldBe("Hello world...");
        }

        [Fact]
        public async Task Should_Wrap_SimpleJson_By_Default()
        {
            // Act
            var response = await GetSuccessResponseBodyAsString("/Test/SimpleJson");

            var ajaxResponse = JsonConvert.DeserializeObject<MvcAjaxResponse<SimpleViewModel>>(response, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            ajaxResponse.Result.StrValue.ShouldBe("Forty Two");
            ajaxResponse.Result.IntValue.ShouldBe(42);
        }

        private async Task<string> GetSuccessResponseBodyAsString(string url)
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}