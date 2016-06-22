using System;
using System.Net.Http;
using Abp.Dependency;
using Abp.TestBase.Runtime.Session;
using Microsoft.AspNetCore.Hosting;
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
            var builder = new WebHostBuilder()
                .UseStartup<TStartup>();

            Server = new TestServer(builder);

            ServiceProvider = Server.Host.Services;
            IocManager = ServiceProvider.GetRequiredService<IIocManager>();
            AbpSession = ServiceProvider.GetRequiredService<TestAbpSession>();

            Client = Server.CreateClient();
        }
    }
}
