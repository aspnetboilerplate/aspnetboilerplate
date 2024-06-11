using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class DateTimeFormatterControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private const string BaseUrl = "/api/date-time-format/";

    public DateTimeFormatterControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("tr", "14.02.2024")]
    [InlineData("en-US", "2/14/2024")]
    [InlineData("en-GB", "14/02/2024")]
    public async Task DateTimeFormat_Controller_Test(string culture, string datetime)
    {
        var client = _factory.CreateClient();

        // Arrange
        client.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, culture);
        var content = new StringContent("{\"TestDate\": \"" + datetime + "\" }", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/services/app/ModelBinding/TestDate", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var output = JsonSerializer.Deserialize<AjaxResponse<string>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        output.Result.Substring(0, datetime.Length).ShouldBe(datetime);
    }
    
    [Theory]
    [InlineData("tr", "14.02.2024")]
    [InlineData("en-US", "2/14/2024")]
    [InlineData("en-GB", "14/02/2024")]
    public async Task DateTimeFormat_Controller_Nullable_Test(string culture, string datetime)
    {
        var client = _factory.CreateClient();

        // Arrange
        client.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, culture);
        var content = new StringContent("{\"TestDate\": \"" + datetime + "\" }", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/services/app/ModelBinding/NullableTestDate", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var output = JsonSerializer.Deserialize<AjaxResponse<string>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        output.Result.Substring(0, datetime.Length).ShouldBe(datetime);
    }
    
    [Theory]
    [InlineData("tr", "14.02.2024")]
    [InlineData("en-US", "2/14/2024")]
    [InlineData("en-GB", "14/02/2024")]
    public async Task DateTimeFormat_Controller_DateOnly_Test(string culture, string datetime)
    {
        var client = _factory.CreateClient();

        // Arrange
        client.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, culture);
        var content = new StringContent("{\"TestDate\": \"" + datetime + "\" }", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/services/app/ModelBinding/DateOnlyTestDate", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var output = JsonSerializer.Deserialize<AjaxResponse<string>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        output.Result.Substring(0, datetime.Length).ShouldBe(datetime);
    }
}