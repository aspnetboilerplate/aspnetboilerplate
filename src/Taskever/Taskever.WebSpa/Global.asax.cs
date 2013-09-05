using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Web.Controllers;
using Abp.Web.Controllers.Dynamic;
using Taskever.Web.Api;
using Taskever.Web.App_Start;
using Taskever.Web.Dependency;

namespace Taskever.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private TaskeverBootstrapper _bootstrapper;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            _bootstrapper = new TaskeverBootstrapper();
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