using System.Web.Security;
using Abp.Security;
using Abp.Startup.Web;

namespace Taskever.Web.Mvc
{
    public class MvcApplication : AbpWebApplication
    {
        protected override void Application_AuthenticateRequest(object sender, System.EventArgs e)
        {
            base.Application_AuthenticateRequest(sender, e);

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
