using System;
using System.Web;
using Abp.Startup;

namespace Abp.Web.Startup
{
    /// <summary>
    /// This class is used to start the web application
    /// </summary>
    public abstract class AbpWebApplication : HttpApplication
    {
        protected AbpBootstrapper AbpBootstrapper;

        protected virtual void Application_Start()
        {
            AbpBootstrapper = new AbpBootstrapper();
            AbpBootstrapper.Initialize();
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            AbpBootstrapper.Dispose();
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }
    }
}
