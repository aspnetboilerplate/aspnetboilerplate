using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
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
            //var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (authCookie == null)
            //{
            //    return;
            //}

            //var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //var roles = authTicket.UserData.Split(new[] { '|' });
            //var userIdentity = new GenericIdentity(authTicket.Name);
            //var userPrincipal = new GenericPrincipal(userIdentity, roles);

            //Context.User = userPrincipal;
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
