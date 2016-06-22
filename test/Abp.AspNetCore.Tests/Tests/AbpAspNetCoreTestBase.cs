using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.AspNetCore.App;
using Abp.AspNetCore.Tests.Infrastructure;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;

namespace Abp.AspNetCore.Tests
{
    public abstract class AbpAspNetCoreTestBase
    {
        protected TestServer Server { get; }

        protected HttpClient Client { get; }

        protected IIocManager IocManager { get; }

        protected TestAbpSession AbpSession { get; }

        protected AbpAspNetCoreTestBase()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();

            Server = new TestServer(builder);

            IocManager = Server.Host.Services.GetRequiredService<IIocManager>();
            AbpSession = Server.Host.Services.GetRequiredService<TestAbpSession>();

            Client = Server.CreateClient();
        }
        
        protected async Task<T> GetResponseAsObjectAsync<T>(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var strResponse = await GetResponseAsStringAsync(url, expectedStatusCode);
            return JsonConvert.DeserializeObject<T>(strResponse, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        protected async Task<string> GetResponseAsStringAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await GetResponseAsync(url, expectedStatusCode);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> GetResponseAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await Client.GetAsync(url);
            response.StatusCode.ShouldBe(expectedStatusCode);
            return response;
        }
    }
}
