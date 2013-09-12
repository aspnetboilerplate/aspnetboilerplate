using System;
using Abp.Startup;

namespace Abp.Web.Mvc.Startup
{
    public class AbpWebApplication : System.Web.HttpApplication
    {
        private AbpBootstrapper _bootstrapper;

        protected void Application_Start()
        {
            _bootstrapper = new AbpBootstrapper();
            _bootstrapper.Initialize();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _bootstrapper.Dispose();
        }

    }
}
