using System;
using Abp.Threading;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore
{
    //TODO: Inject all available services?

    public abstract class AbpStartup : IDisposable
    {
        protected AbpBootstrapper AbpBootstrapper { get; private set; }

        protected AbpStartup(IHostingEnvironment env)
        {
            ThreadCultureSanitizer.Sanitize();

            AbpBootstrapper = new AbpBootstrapper();
            AbpBootstrapper.Initialize();
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
            //AbpBootstrapper.Dispose(); //TODO: Dispose?
        }
    }
}
