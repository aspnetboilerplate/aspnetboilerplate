using System;
using System.Globalization;
using System.Linq;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources.Xml;
using Abp.Threading;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
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
            ConfigureRequestLocalization(app);
        }

        protected virtual void ConfigureRequestLocalization(IApplicationBuilder app)
        {
            using (var languageManager = AbpBootstrapper.IocManager.ResolveAsDisposable<ILanguageManager>())
            {
                var supportedCultures = languageManager.Object
                    .GetLanguages()
                    .Select(l => new CultureInfo(l.Name))
                    .ToArray();

                var defaultCulture = new RequestCulture(
                    languageManager.Object
                        .GetLanguages()
                        .FirstOrDefault(l => l.IsDefault)
                        ?.Name
                );

                var options = new RequestLocalizationOptions
                {
                    DefaultRequestCulture = defaultCulture,
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                };

                app.UseRequestLocalization(options);
            }
        }

        public void Dispose()
        {
            //AbpBootstrapper.Dispose(); //TODO: Dispose?
        }
    }
}
