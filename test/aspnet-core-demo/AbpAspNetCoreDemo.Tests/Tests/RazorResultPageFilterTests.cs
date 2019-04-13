using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class RazorResultPageFilterTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public RazorResultPageFilterTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task RazorPage_RazorResultPageFilter_Get_Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/ResultFilterPageDemo");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            result.ShouldBe("OnGet");
        }

        [Fact]
        public async Task RazorPage_RazorResultPageFilter_Post_Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/ResultFilterPageDemo", new StringContent(""));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<AjaxResponse>(await response.Content.ReadAsStringAsync());

            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            result.Result.ShouldBe("OnPost");
        }
    }
}
