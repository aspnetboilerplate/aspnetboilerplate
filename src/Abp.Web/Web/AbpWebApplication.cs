using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Abp.Collections.Extensions;
using Abp.Localization;
using Abp.Modules;
using Abp.Threading;
using Abp.Web.Configuration;

namespace Abp.Web
{
    /// <summary>
    /// This class is used to simplify starting of ABP system using <see cref="AbpBootstrapper"/> class..
    /// Inherit from this class in global.asax instead of <see cref="HttpApplication"/> to be able to start ABP system.
    /// </summary>
    /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</typeparam>
    public abstract class AbpWebApplication<TStartupModule> : HttpApplication
        where TStartupModule : AbpModule
    {
        /// <summary>
        /// Gets a reference to the <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        public static AbpBootstrapper AbpBootstrapper { get; } = AbpBootstrapper.Create<TStartupModule>();
        private static IAbpWebLocalizationConfiguration _webLocalizationConfiguration;

        /// <summary>
        /// This method is called by ASP.NET system on web application's startup.
        /// </summary>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            ThreadCultureSanitizer.Sanitize();

            AbpBootstrapper.Initialize();

            _webLocalizationConfiguration = AbpBootstrapper.IocManager.Resolve<IAbpWebLocalizationConfiguration>();
        }

        /// <summary>
        /// This method is called by ASP.NET system on web application shutdown.
        /// </summary>
        protected virtual void Application_End(object sender, EventArgs e)
        {
            AbpBootstrapper.Dispose();
        }

        /// <summary>
        /// This method is called by ASP.NET system when a session starts.
        /// </summary>
        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a session ends.
        /// </summary>
        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a request starts.
        /// </summary>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            SetCurrentCulture();
        }

        protected virtual void SetCurrentCulture()
        {
            var langCookie = Request.Cookies[_webLocalizationConfiguration.CookieName];
            if (langCookie != null && GlobalizationHelper.IsValidCultureCode(langCookie.Value))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(langCookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCookie.Value);
            }
            else if (!Request.UserLanguages.IsNullOrEmpty())
            {
                var firstValidLanguage = Request?.UserLanguages?.FirstOrDefault(GlobalizationHelper.IsValidCultureCode);
                if (firstValidLanguage != null)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(firstValidLanguage);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(firstValidLanguage);
                }
            }
        }

        /// <summary>
        /// This method is called by ASP.NET system when a request ends.
        /// </summary>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {

        }
    }
}
