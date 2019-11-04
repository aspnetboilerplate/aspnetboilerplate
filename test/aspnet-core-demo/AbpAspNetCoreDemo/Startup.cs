using System;
using System.IO;
using System.Linq;
using System.Threading;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Json;
using Abp.PlugIns;
using AbpAspNetCoreDemo.Controllers;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.SubSystems.Conversion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace AbpAspNetCoreDemo
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public static readonly AsyncLocal<IocManager> IocManager = new AsyncLocal<IocManager>();

        public Startup(IWebHostEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            //Some test classes
            services.AddTransient<MyTransientClass1>();
            services.AddTransient<MyTransientClass2>();
            services.AddScoped<MyScopedClass>();

            //Add framework services
            services.AddMvc(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Value)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

            // Waiting for OData .NET Core 3.0 support, see https://github.com/OData/WebApi/issues/1748
            // services.AddOData();

            // Workaround: https://github.com/OData/WebApi/issues/1177
            // Waiting for OData .NET Core 3.0 support, see https://github.com/OData/WebApi/issues/1748
            //services.AddMvcCore(options =>
            //{
            //    foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
            //    {
            //        outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            //    }

            //    foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
            //    {
            //        inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            //    }
            //});

            //Configure Abp and Dependency Injection. Should be called last.
            return services.AddAbp<AbpAspNetCoreDemoModule>(options =>
            {
                options.IocManager = IocManager.Value ?? new IocManager();

                options.PlugInSources.Add(
                    new AssemblyFileListPlugInSource(
                        Path.Combine(_env.ContentRootPath, @"..\AbpAspNetCoreDemo.PlugIn\bin\Debug\netcoreapp3.0\AbpAspNetCoreDemo.PlugIn.dll")
                    )
                );

                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );

                var propInjector = options.IocManager.IocContainer.Kernel.ComponentModelBuilder
                    .Contributors
                    .OfType<PropertiesDependenciesModelInspector>()
                    .Single();

                options.IocManager.IocContainer.Kernel.ComponentModelBuilder.RemoveContributor(propInjector);
                options.IocManager.IocContainer.Kernel.ComponentModelBuilder.AddContributor(new AbpPropertiesDependenciesModelInspector(new DefaultConversionManager()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); //Initializes ABP framework. Should be called first.

            // Waiting for OData .NET Core 3.0 support, see https://github.com/OData/WebApi/issues/1748
            // app.UseOData(builder =>
            // {
            //     builder.EntitySet<Product>("Products").EntityType.Expand().Filter().OrderBy().Page().Select();
            // });

            // Return IQueryable from controllers
            //app.UseUnitOfWork(options =>
            //{
            //    options.Filter = httpContext => httpContext.Request.Path.Value.StartsWith("/odata");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseEmbeddedFiles(); //Allows to expose embedded files to the web!

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>().EndpointConfiguration.ConfigureAllEndpoints(endpoints);

                //TODO@3.0 related: https://github.com/OData/WebApi/issues/1707
                //routes.MapODataServiceRoute(app); ???
            });
        }
    }
}
