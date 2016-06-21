using System.Globalization;
using System.Linq;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources.Xml;
using Castle.LoggingFacility.MsLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore
{
    public static class AbpApplicationBuilderExtensions
    {
        public static void UseAbp(this IApplicationBuilder app)
        {
            SetXmlRootDirectory(app);
            AddCastleLoggerFactory(app);

            InitializeAbp(app);

            ConfigureRequestLocalization(app);
        }

        private static void InitializeAbp(IApplicationBuilder app)
        {
            var abpBootstrapper = app.ApplicationServices.GetRequiredService<AbpBootstrapper>();
            abpBootstrapper.Initialize();
        }

        private static void SetXmlRootDirectory(IApplicationBuilder app)
        {
            var hostingEnvironment = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            XmlLocalizationSource.RootDirectoryOfApplication = hostingEnvironment.WebRootPath;
        }

        private static void AddCastleLoggerFactory(IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var castleLoggerFactory = app.ApplicationServices.GetRequiredService<Castle.Core.Logging.ILoggerFactory>();
            loggerFactory.AddCastleLogger(castleLoggerFactory);
        }

        private static void ConfigureRequestLocalization(IApplicationBuilder app)
        {
            using (var languageManager = app.ApplicationServices.GetRequiredService<IIocResolver>().ResolveAsDisposable<ILanguageManager>())
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
    }
}
