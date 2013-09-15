using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Taskever.Web.Areas.Abp.Models;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Areas.Abp.Controllers
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
    }
}
