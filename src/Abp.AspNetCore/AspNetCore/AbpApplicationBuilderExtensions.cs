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
using System.Globalization;

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
                //TODO: This should be added later than authorization middleware!
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

        public static void UseAbpRequestLocalization(this IApplicationBuilder app, Action<RequestLocalizationOptions> optionsAction = null)
        {
            var iocResolver = app.ApplicationServices.GetRequiredService<IIocResolver>();
            using (var languageManager = iocResolver.ResolveAsDisposable<ILanguageManager>())
            {
                var supportedCultures = languageManager.Object
                    .GetLanguages()
                    .Select(l => CultureInfo.GetCultureInfo(l.Name))
                    .ToArray();

                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                };

                var userProvider = new AbpUserRequestCultureProvider();
                
                //0: QueryStringRequestCultureProvider
                options.RequestCultureProviders.Insert(1, userProvider);
                options.RequestCultureProviders.Insert(2, new AbpLocalizationHeaderRequestCultureProvider());
                //3: CookieRequestCultureProvider
                options.RequestCultureProviders.Insert(4, new AbpDefaultRequestCultureProvider());
                //5: AcceptLanguageHeaderRequestCultureProvider

                optionsAction?.Invoke(options);

                userProvider.CookieProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
                userProvider.HeaderProvider = options.RequestCultureProviders.OfType<AbpLocalizationHeaderRequestCultureProvider>().FirstOrDefault();

                app.UseRequestLocalization(options);
            }
        }

        public static void AddSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                /*X-Content-Type-Options header tells the browser to not try and “guess” what a mimetype of a resource might be, and to just take what mimetype the server has returned as fact.*/
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
              
                /*X-XSS-Protection is a feature of Internet Explorer, Chrome and Safari that stops pages from loading when they detect reflected cross-site scripting (XSS) attacks*/
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
               
                /*The X-Frame-Options HTTP response header can be used to indicate whether or not a browser should be allowed to render a page in a <frame>, <iframe> or <object>. SAMEORIGIN makes it being displayed in a frame on the same origin as the page itself. The spec leaves it up to browser vendors to decide whether this option applies to the top level, the parent, or the whole chain*/
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
              
                await next();
            });
        }

    }
}
