using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class RazorExceptionPageFilterTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public RazorExceptionPageFilterTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task RazorPage_RazorExceptionPageFilter_Get_Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/ExceptionFilterPageDemo");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

            var result = JsonConvert.DeserializeObject<AjaxResponse>(await response.Content.ReadAsStringAsync());

            result.ShouldNotBeNull();
            result.Error.Message.ShouldBe("OnGet");
         }

        [Fact]
        public async Task RazorPage_RazorExceptionPageFilter_Post_Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/ExceptionFilterPageDemo", new StringContent(""));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

            var result = await response.Content.ReadAsStringAsync();

            result.IndexOf("UserFriendlyException: OnPost",
                StringComparison.InvariantCultureIgnoreCase).ShouldNotBe(-1);
        }
    }
}
