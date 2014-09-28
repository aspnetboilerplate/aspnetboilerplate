using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Abp.Dependency;
using Abp.Localization;
using Abp.Reflection;
using Abp.Startup;
using Castle.MicroKernel.Registration;

namespace Abp.Web
{
    /// <summary>
    /// This class is used to simplify starting of ABP system using <see cref="AbpBootstrapper"/> class..
    /// Inherit from this class in global.asax instead of <see cref="HttpApplication"/> to be able to start ABP system.
    /// </summary>
    public abstract class AbpWebApplication : HttpApplication
    {
        /// <summary>
        /// Gets a reference to the <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        private AbpBootstrapper AbpBootstrapper { get; set; }

        /// <summary>
        /// This method is called by ASP.NET system on web application's startup.
        /// </summary>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            IocManager.Instance.IocContainer.Register(Component.For<IAssemblyFinder>().ImplementedBy<WebAssemblyFinder>());

            AbpBootstrapper = new AbpBootstrapper();
            AbpBootstrapper.Initialize();
        }

        /// <summary>
        /// This method is called by ASP.NET system on web application's end.
        /// </summary>
        protected virtual void Application_End(object sender, EventArgs e)
        {
            AbpBootstrapper.Dispose();
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            var langCookie = Request.Cookies["Abp.Localization.CultureName"];
            if (langCookie != null && GlobalizationHelper.IsValidCultureCode(langCookie.Value))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(langCookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCookie.Value);
            }
        }

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// This method is called by ASP.NET system on an authentication request.
        /// </summary>
        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        //TODO: Implement all events!
    }
}
