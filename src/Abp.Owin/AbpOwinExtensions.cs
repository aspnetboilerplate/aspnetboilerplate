using System;
using System.Web;
using Abp.Dependency;
using Abp.Modules;
using Abp.Owin.EmbeddedResources;
using Abp.Resources.Embedded;
using Abp.Threading;
using Abp.Web.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Abp.Owin
{
    /// <summary>
    /// OWIN extension methods for ABP.
    /// </summary>
    public static class AbpOwinExtensions
    {
        /// <summary>
        /// This should be called as the first line for OWIN based applications for ABP framework.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseAbp(this IAppBuilder app)
        {
            app.UseAbp(null);
        }

        public static void UseAbp(this IAppBuilder app, [CanBeNull] Action<AbpOwinOptions> optionsAction)
        {
            ThreadCultureSanitizer.Sanitize();

            var options = new AbpOwinOptions
            {
                UseEmbeddedFiles = HttpContext.Current?.Server != null
            };

            optionsAction?.Invoke(options);

            if (options.UseEmbeddedFiles)
            {
                if (HttpContext.Current?.Server == null)
                {
                    throw new AbpInitializationException("Can not enable UseEmbeddedFiles for OWIN since HttpContext.Current is null! If you are using ASP.NET Core, serve embedded resources through ASP.NET Core middleware instead of OWIN. See http://www.aspnetboilerplate.com/Pages/Documents/Embedded-Resource-Files#aspnet-core-configuration");
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileSystem = new AbpOwinEmbeddedResourceFileSystem(
                        IocManager.Instance.Resolve<IEmbeddedResourceManager>(),
                        IocManager.Instance.Resolve<IWebEmbeddedResourcesConfiguration>(),
                        HttpContext.Current.Server.MapPath("~/")
                    )
                });
            }
        }

        /// <summary>
        /// Use this extension method if you don't initialize ABP in other way.
        /// Otherwise, use <see cref="UseAbp(IAppBuilder)"/>.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseAbp<TStartupModule>(this IAppBuilder app)
            where TStartupModule : AbpModule
        {
            app.UseAbp<TStartupModule>(null, null);
        }

        /// <summary>
        /// Use this extension method if you don't initialize ABP in other way.
        /// Otherwise, use <see cref="UseAbp(IAppBuilder)"/>.
        /// </summary>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseAbp<TStartupModule>(this IAppBuilder app, [CanBeNull] Action<AbpBootstrapper> configureAction, [CanBeNull] Action<AbpOwinOptions> optionsAction = null)
            where TStartupModule : AbpModule
        {
            app.UseAbp(optionsAction);

            if (!app.Properties.ContainsKey("_AbpBootstrapper.Instance"))
            {
                var abpBootstrapper = AbpBootstrapper.Create<TStartupModule>();
                app.Properties["_AbpBootstrapper.Instance"] = abpBootstrapper;
                configureAction?.Invoke(abpBootstrapper);
                abpBootstrapper.Initialize();
            }
        }
    }
}