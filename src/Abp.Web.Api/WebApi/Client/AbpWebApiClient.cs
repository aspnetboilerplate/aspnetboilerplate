using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi.Client
{
    public class AbpWebApiClient : ITransientDependency, IAbpWebApiClient
    {
        public static TimeSpan DefaultTimeout { get; set; }

        public string BaseUrl { get; set; }

        public TimeSpan Timeout { get; set; }

        static AbpWebApiClient()
        {
            DefaultTimeout = TimeSpan.FromSeconds(90);
        }

        public AbpWebApiClient()
        {
            Timeout = DefaultTimeout;
        }

        public async Task PostAsync(string url, int? timeout = null)
        {
            await PostAsync<object>(url, timeout);
        }

        public async Task PostAsync(string url, object input, int? timeout = null)
        {
            await PostAsync<object>(url, input, timeout);
        }

        public async Task<TResult> PostAsync<TResult>(string url, int? timeout = null)
            where TResult : class, new()
        {
            return await PostAsync<TResult>(url, null, timeout);
        }

        public async Task<TResult> PostAsync<TResult>(string url, object input, int? timeout = null)
            where TResult : class, new()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = timeout.HasValue ? TimeSpan.FromMilliseconds(timeout.Value) : Timeout;

                if (!BaseUrl.IsNullOrEmpty())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var requestContent = new StringContent(Object2JsonString(input), Encoding.UTF8, "application/json"))
                {
                    using (var response = await client.PostAsync(url, requestContent))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new AbpException("Could not made request to " + url + "! StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                        }

                        var ajaxResponse = JsonString2Object<AjaxResponse<TResult>>(await response.Content.ReadAsStringAsync());
                        if (!ajaxResponse.Success)
                        {
                            throw new AbpRemoteCallException(ajaxResponse.Error);
                        }

                        return ajaxResponse.Result;
                    }
                }
            }
        }

        private static string Object2JsonString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        private static TObj JsonString2Object<TObj>(string str)
        {
            return JsonConvert.DeserializeObject<TObj>(str,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}