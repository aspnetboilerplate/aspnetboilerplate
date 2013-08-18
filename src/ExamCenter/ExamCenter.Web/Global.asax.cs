using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Security;
using System.Web.SessionState;
using Abp.Web.Dependency;
using Abp.Web.Startup;
using Castle.Windsor;
using ExamCenter.Web.App_Start;

namespace ExamCenter.Web
{
    public class Global : System.Web.HttpApplication
    {
        private ExamCenterBootstrapper _bootstrapper;
        
        protected void Application_Start(object sender, EventArgs e)
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _bootstrapper = new ExamCenterBootstrapper();

            //ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(WindsorContainer.Kernel));
            //ApiControllerBuilder

            _bootstrapper.Initialize();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            _bootstrapper.Dispose();
        }
    }
}