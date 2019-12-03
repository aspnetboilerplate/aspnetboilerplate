using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class RazorUowPageFilterTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public RazorUowPageFilterTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/UowFilterPageDemo", "200", "Razor page unit of work filter example")]
        [InlineData("/UowFilterPageDemo2", "500", "Current UnitOfWork is null")]
        public async Task RazorPage_UowPageFilter_Get_Test(string url, string responseCode, string responseText)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>(responseCode));

            var result = await response.Content.ReadAsStringAsync();
            result.ShouldContain(responseText);
        }

        [Theory]
        [InlineData("/UowFilterPageDemo", "500", "Current UnitOfWork is null")]
        [InlineData("/UowFilterPageDemo2", "500", "Current UnitOfWork is null")]
        public async Task RazorPage_UowPageFilter_Post_Test(string url, string responseCode, string responseText)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync(url, new StringContent(""));

            // Assert
            response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>(responseCode));

            var result = await response.Content.ReadAsStringAsync();
            result.ShouldContain(responseText);
        }


        [Theory]
        [InlineData("Get")]
        [InlineData("Post")]
        public async Task RazorPage_UowPageFilter_NoAction_Test(string method)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), "/UowFilterPageDemo3");
            var response = await client.SendAsync(requestMessage);

            // Assert
            response.EnsureSuccessStatusCode();
            (await response.Content.ReadAsStringAsync()).ShouldContain("<title>UowFilterPageDemo3</title>");
        }
    }
}
