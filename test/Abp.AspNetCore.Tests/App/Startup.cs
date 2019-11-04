using System;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.TestBase;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore.App
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var mvc = services.AddMvc()
                    .AddXmlSerializerFormatters();

            mvc.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AbpAspNetCoreModule).GetAssembly()));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
            }).AddCookie();

            //Configure Abp and Dependency Injection
            return services.AddAbp<AppModule>(options =>
            {
                //Test setup
                options.SetupTest();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); //Initializes ABP framework.

            app.UseRouting();

            app.UseAuthentication();
            app.UseAbpAuthorizationExceptionHandling();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>()
                                        .EndpointConfiguration
                                        .ConfigureAllEndpoints(endpoints);
            });
        }
    }
}
