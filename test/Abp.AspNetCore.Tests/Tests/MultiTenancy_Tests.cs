using System;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Configuration.Startup;
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
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public MultiTenancy_Tests()
        {
            _multiTenancyConfig = IocManager.Resolve<IMultiTenancyConfig>();
            _multiTenancyConfig.IsEnabled = true;
            _multiTenancyConfiguration = Resolve<IWebMultiTenancyConfiguration>();
        }

        [Fact]
        public async Task HttpHeaderTenantResolveContributor_Test()
        {
            Client.DefaultRequestHeaders.Add(_multiTenancyConfig.TenantIdResolveKey, "42");

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
        public async Task HttpHeaderTenantResolveContributor_Configure_Test()
        {
            _multiTenancyConfig.TenantIdResolveKey = "Abp-TenantId";

            Client.DefaultRequestHeaders.Add(_multiTenancyConfig.TenantIdResolveKey, "42");

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
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_multiTenancyConfig.TenantIdResolveKey, "42").ToString());

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
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_multiTenancyConfig.TenantIdResolveKey, "43").ToString());
            Client.DefaultRequestHeaders.Add(_multiTenancyConfig.TenantIdResolveKey, "42");

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
        [InlineData("http://{TENANCY_NAME}.mysite.com", "http://default.mysite.com", 1)]
        [InlineData("http://{TENANCY_NAME}.mysite.com:8080", "http://default.mysite.com:8080", 1)]
        [InlineData("http://{TENANCY_NAME}.mysite.com/", "http://default.mysite.com/", 1)]
        [InlineData("http://{TENANCY_NAME}.mysite.com/host", "http://default.mysite.com/host", 1)]
        [InlineData("http://{TENANCY_NAME}:80", "http://default:80", 1)]
        [InlineData("http://{TENANCY_NAME}:80", "http://test:80", null)]
        [InlineData("http://{TENANCY_NAME}.mysite.com/host", "http://mysite.default.com/host", null)]
        public async Task DomainTenantResolveContributor_Test(string domainFormat, string domain, int? tenantId)
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
            response.Result.ShouldBe(tenantId);
        }
    }
}
