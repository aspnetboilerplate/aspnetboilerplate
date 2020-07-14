using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class AbpLocalizationControllerTests : AppTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-us")]
        public async Task Should_Set_Culture_Cookie(string cultureName)
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<AbpLocalizationController>(
                    nameof(AbpLocalizationController.ChangeCulture),
                    new { cultureName }
                ),
                HttpStatusCode.Redirect
            );

            // Assert
            response.Headers.Location.IsAbsoluteUri.ShouldBeFalse();
            response.Headers.Location.ToString().ShouldBe("/");

            var exceptedCookieValue = $"c={cultureName}|uic={cultureName}";
            var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            cookies.Single().ShouldContain($".AspNetCore.Culture={WebUtility.UrlEncode(exceptedCookieValue)}");
        }

        [Theory]
        [InlineData("english")]
        [InlineData("")]
        public async Task Should_Throw_Invalid_Culture_Name(string cultureName)
        {
            // Act
            var exception = await Should.ThrowAsync<AbpException>(async () => {
                await GetResponseAsync(
                    GetUrl<AbpLocalizationController>(
                        nameof(AbpLocalizationController.ChangeCulture),
                        new { cultureName }
                    )
                );
            });
            
            // Assert
            exception.Message.ShouldBe($"Unknown language: {cultureName}. It must be a valid culture!");
        }

        [Theory]
        [InlineData("/local/site", "/local/site")]
        [InlineData("/local/site?id=1", "/local/site?id=1")] //aspnetcore can only bind the first query param in an unescaped return url
        [InlineData("http://localhost/page", "/page")]
        [InlineData("http%3A%2F%2Flocalhost%2Fpage", "/page")]
        [InlineData("%2Flocal%2Fsite%3Fid%3D1%26value%3D2", "/local/site?id=1&value=2")]
        [InlineData("%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1", "/経営?仕事場=business&ID=1")]
        public async Task Should_Redirect_Local_Return_Url(string returnUrl, string expected)
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<AbpLocalizationController>(
                    nameof(AbpLocalizationController.ChangeCulture),
                    new { cultureName = "en", returnUrl }
                ),
                HttpStatusCode.Redirect
            );

            // Assert
            response.Headers.Location.IsAbsoluteUri.ShouldBeFalse();
            response.Headers.Location.ToString().ShouldBe(expected);
        }

        [Theory]
        [InlineData("www.example.com", "/")]
        [InlineData("www.example.com/local/site", "/")]
        [InlineData("www.example.com%2Flocal%2Fsite%3Fid%3D1", "/")]
        [InlineData("http%3A%2F%2Fwww.example.com%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1", "/")]
        [InlineData("%252fAccount%252fLogin", "/")]
        public async Task Should_Redirect_External_Return_Url(string returnUrl, string expected)
        {
            // Act
            var response = await GetResponseAsync(
                GetUrl<AbpLocalizationController>(
                    nameof(AbpLocalizationController.ChangeCulture),
                    new { cultureName = "en", returnUrl }
                ),
                HttpStatusCode.Redirect
            );

            // Assert
            response.Headers.Location.IsAbsoluteUri.ShouldBeFalse();
            response.Headers.Location.ToString().ShouldBe(expected);
        }
    }
}