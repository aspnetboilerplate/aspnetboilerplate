using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Abp.AspNetCore.Tests
{
    public class AbpAspNetCoreTestBase
    {
        protected TestServer Server { get; }
        protected HttpClient Client { get; }

        public AbpAspNetCoreTestBase()
        {
            // Arrange
            var builder = new WebHostBuilder().UseStartup<Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }
    }
}
