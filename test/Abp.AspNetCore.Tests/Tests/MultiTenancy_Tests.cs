using System;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Web.Models;
using Abp.Web.MultiTenancy;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class MultiTenancy_Tests : AppTestBase
    {
        private readonly IWebMultiTenancyConfiguration _multiTenancyConfiguration;

        public MultiTenancy_Tests()
        {
            IocManager.Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _multiTenancyConfiguration = Resolve<IWebMultiTenancyConfiguration>();
        }

        [Fact]
        public async Task HttpHeaderTenantResolveContributor_Test()
        {
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task HttpCookieTenantResolveContributor_Test()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "42").ToString());

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task Header_Should_Have_High_Priority_Than_Cookie()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "43").ToString());
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Theory]
        [InlineData("http://{TENANCY_NAME}.mysite.com", "http://default.mysite.com")]
        [InlineData("http://{TENANCY_NAME}.mysite.com:8080", "http://default.mysite.com:8080")]
        [InlineData("http://{TENANCY_NAME}.mysite.com/", "http://default.mysite.com/")]
        public async Task DomainTenantResolveContributor_Test(string domainFormat, string domain)
        {
            _multiTenancyConfiguration.DomainFormat = domainFormat;
            Client.BaseAddress = new Uri(domain);

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(1);
        }
    }
}
