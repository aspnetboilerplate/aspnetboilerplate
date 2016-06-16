using System;
using Abp.Localization.Sources.Xml;
using Abp.Threading;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore
{
    //TODO: Inject all available services?

    public abstract class AbpStartup : IDisposable
    {
        protected AbpBootstrapper AbpBootstrapper { get; private set; }

        protected AbpStartup(IHostingEnvironment env, bool initialize = true)
        {
            //TODO: TEST
            XmlLocalizationSource.RootDirectoryOfApplication = env.WebRootPath;

            AbpBootstrapper = new AbpBootstrapper();

            if (initialize)
            {
                InitializeAbp();
            }
        }

        protected virtual void InitializeAbp()
        {
            ThreadCultureSanitizer.Sanitize();
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
