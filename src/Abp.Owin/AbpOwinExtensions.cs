using System;
using Abp.Modules;
using Abp.Threading;
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
            ThreadCultureSanitizer.Sanitize();
        }

        /// <summary>
        /// Use this extension method if you don't initialize ABP in other way.
        /// Otherwise, use <see cref="UseAbp"/>.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseAbp<TStartupModule>(this IAppBuilder app)
            where TStartupModule : AbpModule
        {
            app.UseAbp<TStartupModule>(abpBootstrapper => { });
        }

        /// <summary>
        /// Use this extension method if you don't initialize ABP in other way.
        /// Otherwise, use <see cref="UseAbp"/>.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configureAction"></param>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseAbp<TStartupModule>(this IAppBuilder app, Action<AbpBootstrapper> configureAction)
            where TStartupModule : AbpModule
        {
            app.UseAbp();

            var abpBootstrapper = app.Properties["_AbpBootstrapper.Instance"] as AbpBootstrapper;
            if (abpBootstrapper == null)
            {
                abpBootstrapper = AbpBootstrapper.Create<TStartupModule>();
                configureAction(abpBootstrapper);
                abpBootstrapper.Initialize();
            }
        }
    }
}