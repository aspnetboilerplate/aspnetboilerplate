using System;
using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Filters;
using AbpAspNetCoreDemo.EntityFrameworkCore;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AbpAspNetCoreDemo
{
    public class Startup : AbpStartup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
            : base(env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        protected override void InitializeAbp()
        {
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(
                f => f.UseLog4Net().WithConfig("log4net.config")
                );

            base.InitializeAbp();
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDbContext<MyDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AbpAuthorizationFilter));
                options.Filters.Add(typeof(AbpExeptionFilter));
                options.Filters.Add(typeof(AbpResultFilter));
                options.Filters.Add(typeof(AbpAuditActionFilter));

                //TODO: InputFotmatter!

                options.OutputFormatters.Add(new JsonOutputFormatter(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));

            }).AddControllersAsServices();

            return base.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            base.Configure(app, env, loggerFactory);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc(options =>
            {
                
            });
        }
    }
}
