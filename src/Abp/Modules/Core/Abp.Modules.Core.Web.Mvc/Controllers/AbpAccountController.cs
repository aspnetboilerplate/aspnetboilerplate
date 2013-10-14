using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Mvc.Models;
using Abp.Security;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Models;

namespace Abp.Modules.Core.Mvc.Controllers
{
    /* This class is written by looking at the source codes of System.Web.Mvc.HandleErrorAttribute class */

    public abstract class AbpAccountController : AbpController
    {
        private readonly IUserService _userService;

        protected AbpAccountController(IUserService userService)
        {
            _userService = userService;
        }

        public virtual ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public virtual JsonResult Login(LoginModel loginModel)
        {
            //Thread.Sleep(5000);
            //try
            //{
            if (ModelState.IsValid)
            {
                if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                {
                    throw new AbpUserFriendlyException("No user name or password!");
                }

                //FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);
                var user = _userService.GetUserOrNull(loginModel.EmailAddress, loginModel.Password);
                var identity = new AbpIdentity(1, user.Id, user.EmailAddress);
                var authTicket = new FormsAuthenticationTicket(1, loginModel.EmailAddress, DateTime.Now, DateTime.Now.AddMinutes(15), false, identity.SerializeToString());
                var encTicket = FormsAuthentication.Encrypt(authTicket);
                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                return Json(new AbpMvcAjaxResult(true) { TargetUrl = "/" });
            }

            return Json(new AbpMvcAjaxResult(new AbpErrorInfo("Your form is invalid!")));
            //}
            //catch (AbpUserFriendlyException ex)
            //{
            //    //TODO: log ex as warning
            //    return Json(new AbpMvcAjaxResult(new AbpErrorInfo(ex.Message)));
            //}
            //catch (Exception ex)
            //{
            //    //TODO: log ex as Error
            //    Logger.Error(ex.Message, ex);
            //    return Json(new AbpMvcAjaxResult(new AbpErrorInfo("System error!")));
            //}
        }

        [Authorization.AbpAuthorize]
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public virtual JsonResult Register(RegisterUserInput registerUser)
        {
            _userService.RegisterUser(registerUser);
            return Login(new LoginModel { EmailAddress = registerUser.EmailAddress, Password = registerUser.Password });
        }
    }
}
