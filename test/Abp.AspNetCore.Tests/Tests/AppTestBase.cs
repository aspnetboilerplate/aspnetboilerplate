using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.AspNetCore.App;
using Abp.AspNetCore.TestBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;

namespace Abp.AspNetCore.Tests
{
    public abstract class AppTestBase : AbpAspNetCoreIntegratedTestBase<Startup>
    {
        protected virtual async Task<T> GetResponseAsObjectAsync<T>(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var strResponse = await GetResponseAsStringAsync(url, expectedStatusCode);
            return JsonConvert.DeserializeObject<T>(strResponse, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        protected virtual async Task<string> GetResponseAsStringAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await GetResponseAsync(url, expectedStatusCode);
            return await response.Content.ReadAsStringAsync();
        }

        protected virtual async Task<HttpResponseMessage> GetResponseAsync(string url, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await Client.GetAsync(url);
            response.StatusCode.ShouldBe(expectedStatusCode);
            return response;
        }
    }
}