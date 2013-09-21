using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Web.Mvc.Controllers;
using Taskever.Web.Areas.Abp.Modules.Core.Models;

namespace Taskever.Web.Areas.Abp.Modules.Core.Controllers
{
    public class AccountController : AbpController
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                {
                    throw new UserFriendlyException("No user name or password!");
                }

                FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);
                return Redirect("/Taskever"); //TODO: Implement Return URL!
            }

            return View();
        }
    }
}
