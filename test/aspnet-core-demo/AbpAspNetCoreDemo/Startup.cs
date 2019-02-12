using System;
using System.IO;
using System.Linq;
using System.Threading;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.PlugIns;
using AbpAspNetCoreDemo.Controllers;
using Castle.Facilities.Logging;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.SubSystems.Conversion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AbpAspNetCoreDemo
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public static readonly AsyncLocal<IocManager> IocManager = new AsyncLocal<IocManager>();

        public Startup(IHostingEnvironment env)
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
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            //Configure Abp and Dependency Injection. Should be called last.
            return services.AddAbp<AbpAspNetCoreDemoModule>(options =>
            {
                options.IocManager = IocManager.Value ?? new IocManager();

                options.PlugInSources.Add(
                    new AssemblyFileListPlugInSource(
                        Path.Combine(_env.ContentRootPath, @"..\AbpAspNetCoreDemo.PlugIn\bin\Debug\netstandard2.0\AbpAspNetCoreDemo.PlugIn.dll")
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); //Initializes ABP framework. Should be called first.

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseMvc(routes =>
            {
                app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>().RouteConfiguration.ConfigureAll(routes);
            });
        }
    }
}
