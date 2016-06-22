using System.Net.Http;
using Abp.AspNetCore.Tests.App;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Abp.AspNetCore.Tests
{
    public abstract class AbpAspNetCoreTestBase
    {
        protected TestServer Server { get; }

        protected HttpClient Client { get; }

        protected AbpAspNetCoreTestBase()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();

            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }
    }
}
