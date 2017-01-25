using System.Globalization;
using System.Linq;
using Abp.AspNetCore.Localization;
using Abp.AspNetCore.Mvc.Views;
using Abp.Dependency;
using Abp.Localization;
using Castle.LoggingFacility.MsLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abp.AspNetCore
{
    public static class AbpApplicationBuilderExtensions
    {
        public static void UseAbp(this IApplicationBuilder app)
        {
            AddCastleLoggerFactory(app);

            InitializeAbp(app);

            ConfigureRequestLocalization(app);
        }

        public static void UseEmbeddedFiles(this IApplicationBuilder app)
        {
            //TODO: Can improve it or create a custom middleware?
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new EmbeddedResourceFileProvider(
                        app.ApplicationServices.GetRequiredService<IIocResolver>()
                    )
                }
            );
        }

        private static void InitializeAbp(IApplicationBuilder app)
        {
            var abpBootstrapper = app.ApplicationServices.GetRequiredService<AbpBootstrapper>();
            abpBootstrapper.Initialize();
        }

        private static void AddCastleLoggerFactory(IApplicationBuilder app)
        {
            var castleLoggerFactory = app.ApplicationServices.GetService<Castle.Core.Logging.ILoggerFactory>();
            if (castleLoggerFactory == null)
            {
                return;
            }

            app.ApplicationServices
                .GetRequiredService<ILoggerFactory>()
                .AddCastleLogger(castleLoggerFactory);
        }

        private static void ConfigureRequestLocalization(IApplicationBuilder app)
        {
            var iocResolver = app.ApplicationServices.GetRequiredService<IIocResolver>();
            using (var languageManager = iocResolver.ResolveAsDisposable<ILanguageManager>())
            {
                var defaultLanguage = languageManager.Object
                    .GetLanguages()
                    .FirstOrDefault(l => l.IsDefault);

                if (defaultLanguage == null)
                {
                    return;
                }

                var supportedCultures = languageManager.Object
                    .GetLanguages()
                    .Select(l => new CultureInfo(l.Name))
                    .ToArray();

                var defaultCulture = new RequestCulture(defaultLanguage.Name);

                var options = new RequestLocalizationOptions
                {
                    DefaultRequestCulture = defaultCulture,
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                };

                options.RequestCultureProviders.Insert(0, new AbpLocalizationHeaderRequestCultureProvider());

                app.UseRequestLocalization(options);
            }
        }
    }
}
