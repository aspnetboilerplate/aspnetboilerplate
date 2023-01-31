using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class SanitizerControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private const string BaseUrl = "/SanitizerTest/";

    public SanitizerControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }
    
    [Theory]
    [InlineData("<script>alert('hello')</script>", "alert('hello')")]
    public async Task Sanitizer_Post_Test(string text, string expectedText)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync(BaseUrl + "SanitizeHtml", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("html", text)
        }));

        // Assert

        var result = await response.Content.ReadAsStringAsync();
        var jsonResult = JObject.Parse(result);
        jsonResult["result"].ShouldBe(expectedText);
    }
}