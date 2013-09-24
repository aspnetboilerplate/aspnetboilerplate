using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Abp.Authorization;
using Abp.Startup;

namespace Abp.Web.Mvc.Startup
{
    public class AbpWebApplication : System.Web.HttpApplication
    {
        private AbpBootstrapper _bootstrapper;

        protected virtual void Application_Start()
        {
            _bootstrapper = new AbpBootstrapper();
            _bootstrapper.Initialize();
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

        protected virtual void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            _bootstrapper.Dispose();
        }
    }
}
