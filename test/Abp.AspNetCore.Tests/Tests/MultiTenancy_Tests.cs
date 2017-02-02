using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Web.Models;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class MultiTenancy_Tests : AppTestBase
    {
        public MultiTenancy_Tests()
        {
            IocManager.Resolve<IMultiTenancyConfig>().IsEnabled = true;
        }

        [Fact]
        public async Task HttpHeaderTenantResolveContributer_Test()
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
        public async Task HttpCookieTenantResolveContributer_Test()
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
    }
}
