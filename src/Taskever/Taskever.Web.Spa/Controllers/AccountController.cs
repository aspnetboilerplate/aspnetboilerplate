using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Modules.Core.Authorization;
using Abp.Web.Mvc.Controllers;
using Taskever.Web.Areas.Abp.Modules.Core.Models;

namespace Taskever.Web.Controllers
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
                return Redirect("/"); //TODO: Implement Return URL!
            }

            return View();
        }

        [AbpAuthorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
