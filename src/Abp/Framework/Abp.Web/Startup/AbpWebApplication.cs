using System;
using System.Web;
using System.Web.Security;
using Abp.Security;
using Abp.Startup;

namespace Abp.Web.Mvc.Startup
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
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                return;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userIdentity = new AbpIdentity();
            userIdentity.DeserializeFromString(authTicket.UserData);
            var userPrincipal = new AbpPrincipal(userIdentity);
            Context.User = userPrincipal;
        }
    }
}
