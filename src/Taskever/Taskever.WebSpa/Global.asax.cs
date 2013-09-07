using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Startup;
using Taskever.Web.App_Start;
using RouteConfig = Taskever.Web.App_Start.RouteConfig;

namespace Taskever.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private AbpBootstrapper _bootstrapper;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            _bootstrapper = new AbpBootstrapper();
            _bootstrapper.Initialize();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _bootstrapper.Dispose();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }
    }
}