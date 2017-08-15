using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Proxying;
using Abp.Web.Api.ProxyScripting.Generators.JQuery;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class ProxyScripting_Tests : AppTestBase
    {
        [Fact]// TODO: Getting NotAcceptable return code
        public async Task jQuery_Scripting_Simple_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<AbpServiceProxiesController>(
                    nameof(AbpServiceProxiesController.GetAll),
                    new { type = JQueryProxyScriptGenerator.Name }
                )
            );

            response.ShouldNotBeNullOrEmpty();
        }
    }
}
