using System;
using System.Linq;
using System.Net.Http;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Extensions;
using Abp.TestBase.Runtime.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.TestBase
{
    public abstract class AbpAspNetCoreIntegratedTestBase<TStartup> 
        where TStartup : class
    {
        protected TestServer Server { get; }

        protected HttpClient Client { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected IIocManager IocManager { get; }

        protected TestAbpSession AbpSession { get; }

        protected AbpAspNetCoreIntegratedTestBase()
        {
            var builder = CreateWebHostBuilder();
            Server = CreateTestServer(builder);
            Client = Server.CreateClient();

            ServiceProvider = Server.Host.Services;
            IocManager = ServiceProvider.GetRequiredService<IIocManager>();
            AbpSession = ServiceProvider.GetRequiredService<TestAbpSession>();
        }

        protected virtual IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<TStartup>();
        }

        protected virtual TestServer CreateTestServer(IWebHostBuilder builder)
        {
            return new TestServer(builder);
        }

        protected virtual string GetUrl<TController>()
        {
            var controllerName = typeof(TController).Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName.Left(controllerName.Length - "Controller".Length);
            }

            return "/" + controllerName;
        }

        protected virtual string GetUrl<TController>(string actionName)
        {
            return GetUrl<TController>() + "/" + actionName;
        }

        protected virtual string GetUrl<TController>(string actionName, object queryStringParamsAsAnonymousObject)
        {
            var url = GetUrl<TController>(actionName);

            var dictionary = new RouteValueDictionary(queryStringParamsAsAnonymousObject);
            if (dictionary.Any())
            {
                url += "?" + dictionary.Select(d => $"{d.Key}={d.Value}").JoinAsString("&");
            }

            return url;
        }
    }
}
