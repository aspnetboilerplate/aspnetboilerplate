using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Web.Models;
using AbpAspNetCoreDemo.Core.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class ModelBindingAppServiceTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ModelBindingAppServiceTests()
        {
            _factory = new WebApplicationFactory<Startup>();
        }

        [Theory]
        [InlineData("pt-BR", "42,42", "42,42")]
        [InlineData("en", "42.42", "42.42")]
        [InlineData("en", "4,242", "4242")]
        [InlineData("tr", "42,42", "42,42")]
        [InlineData("en-GB", "42.42", "42.42")]
        public async Task Test_Double_Binding(string culture, string inputPrice, string outputPrice)
        {
            var client = _factory.CreateClient();

            // Arrange
            client.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, culture);
            var content = new StringContent("{Price: \"" + inputPrice + "\" }", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/services/app/ModelBinding/CalculatePrice", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var output = JsonConvert.DeserializeObject<AjaxResponse<CalculatePriceOutput>>(responseContent);

            output.Result.Price.ShouldBe(outputPrice);
            output.Result.Culture.ShouldBe(culture);
        }
    }
}