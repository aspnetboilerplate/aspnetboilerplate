using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Mvc.Controllers;
using Abp.Modules.Core.Mvc.Models;
using Abp.Security;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Models;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
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
            if (ModelState.IsValid)
            {
                if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                {
                    throw new AbpUserFriendlyException("No user name or password!");
                }

                FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);
                var user = _userService.GetActiveUserOrNull(loginModel.EmailAddress, loginModel.Password);
                var identity = new AbpIdentity(1, user.Id, user.EmailAddress);
                var authTicket = new FormsAuthenticationTicket(1, loginModel.EmailAddress, DateTime.Now, DateTime.Now.AddDays(2), true, identity.SerializeToString()); //TODO: true/false?
                var encTicket = FormsAuthentication.Encrypt(authTicket);
                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                return Json(new AbpMvcAjaxResponse { TargetUrl = "/" });
            }

            throw new AbpUserFriendlyException("Your form is invalid!");
        }

        public ActionResult ConfirmEmail(ConfirmEmailInput input)
        {
            _userService.ConfirmEmail(input);
            ViewBag.LoginMessage = "Congratulations! Your account is activated. Enter your email address and password to login";
            return RedirectToAction("Login");
        }

        [Abp.Modules.Core.Authorization.AbpAuthorize]
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ActivationInfo()
        {
            return View();
        }

        public JsonResult Register(RegisterUserInput registerUser)
        {
            //TODO: Return better exception messages!
            //TODO: Show captcha after filling register form, not on startup!

            var recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                throw new AbpUserFriendlyException("Captcha answer cannot be empty.");
            }

            var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                throw new AbpUserFriendlyException("Incorrect captcha answer.");
            }

            registerUser.ProfileImage = ProfileImageHelper.GenerateRandomProfileImage();
            _userService.RegisterUser(registerUser);

            return Json(new AbpMvcAjaxResponse { TargetUrl = Url.Action("ActivationInfo") });
        }
    }
}
