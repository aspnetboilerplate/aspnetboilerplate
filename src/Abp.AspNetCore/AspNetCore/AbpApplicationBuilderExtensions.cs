using System;
using System.Linq;
using Abp.AspNetCore.EmbeddedResources;
using Abp.AspNetCore.Localization;
using Abp.Dependency;
using Abp.Localization;
using Castle.LoggingFacility.MsLogging;
using JetBrains.Annotations;
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
            app.UseAbp(null);
        }

	    public static void UseAbp([NotNull] this IApplicationBuilder app, Action<AbpApplicationBuilderOptions> optionsAction)
	    {
		    Check.NotNull(app, nameof(app));

	        var options = new AbpApplicationBuilderOptions();
            optionsAction?.Invoke(options);

            if (options.UseCastleLoggerFactory)
		    {
			    app.UseCastleLoggerFactory();
			}

			InitializeAbp(app);

		    if (options.UseAbpRequestLocalization)
		    {
			    app.UseAbpRequestLocalization();
		    }
	    }

		public static void UseEmbeddedFiles(this IApplicationBuilder app)
        {
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

        public static void UseCastleLoggerFactory(this IApplicationBuilder app)
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

        public static void UseAbpRequestLocalization(this IApplicationBuilder app)
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
                    .Select(l => CultureInfoHelper.Get(l.Name))
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
