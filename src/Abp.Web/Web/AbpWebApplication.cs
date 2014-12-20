using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Domain.Uow.Web;
using Abp.Localization;
using Abp.Reflection;
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
            //TODO: Use AbpBootstrapper.IocManager
            IocManager.Instance.IocContainer.Register(Component.For<IAssemblyFinder>().ImplementedBy<WebAssemblyFinder>());

            //TODO: Use AbpBootstrapper.IocManager
            //TODO: Move to AbpWebModule?
            IocManager.Instance.IocContainer.Register(Component.For<ICurrentUnitOfWorkProvider>().ImplementedBy<HttpContextCurrentUnitOfWorkProvider>());

            AbpBootstrapper = new AbpBootstrapper();
            AbpBootstrapper.Initialize();
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            AbpBootstrapper.Dispose();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This method is called by ASP.NET system when a request starts.
        /// </summary>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            var langCookie = Request.Cookies["Abp.Localization.CultureName"];
            if (langCookie != null && GlobalizationHelper.IsValidCultureCode(langCookie.Value))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(langCookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCookie.Value);
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
