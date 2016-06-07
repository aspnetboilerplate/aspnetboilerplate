using System;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public abstract class AbpStartup : IDisposable
    {
        protected AbpBootstrapper AbpBootstrapper { get; private set; }

        protected AbpStartup(IHostingEnvironment env)
        {
            AbpBootstrapper = new AbpBootstrapper();
            AbpBootstrapper.Initialize(); //TODO: Here or where?
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return WindsorRegistrationHelper.CreateServiceProvider(AbpBootstrapper.IocManager.IocContainer, services);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
        }

        public void Dispose()
        {
            //AbpBootstrapper.Dispose();
        }
    }
}
