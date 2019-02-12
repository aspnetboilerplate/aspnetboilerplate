using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AbpAspNetCoreDemo.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
               services.BuildServiceProvider();
            });
        }
    }
}
