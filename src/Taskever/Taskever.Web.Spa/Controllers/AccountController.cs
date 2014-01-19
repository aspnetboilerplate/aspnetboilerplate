using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Modules.Core.Mvc.Controllers;
using Abp.Modules.Core.Mvc.Models;
using Abp.Security;
using Abp.Users;
using Abp.Users.Dto;
using Abp.Web.Models;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Models;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Taskever.Web.Models.Account;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpController
    {
        private readonly IUserAppService _userAppService;

        public AccountController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        public virtual ActionResult Login(string returnUrl = "/", string loginMessage = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginMessage = loginMessage;
            return View();
        }

        [HttpPost]
        public virtual JsonResult Login(LoginModel loginModel, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                {
                    throw new AbpUserFriendlyException("No user name or password!");
                }

                FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);
                var user = _userAppService.GetActiveUserOrNull(loginModel.EmailAddress, loginModel.Password);
                var identity = new AbpIdentity(1, user.Id, user.EmailAddress);
                var authTicket = new FormsAuthenticationTicket(1, loginModel.EmailAddress, DateTime.Now, DateTime.Now.AddDays(2), loginModel.RememberMe, identity.SerializeToString()); //TODO: true/false?
                var encTicket = FormsAuthentication.Encrypt(authTicket);
                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                return Json(new AbpMvcAjaxResponse { TargetUrl = returnUrl });
            }

            throw new AbpUserFriendlyException("Your form is invalid!");
        }

        public ActionResult ConfirmEmail(ConfirmEmailInput input)
        {
            _userAppService.ConfirmEmail(input);
            return RedirectToAction("Login", new { loginMessage = "Congratulations! Your account is activated. Enter your email address and password to login" });
        }

        [Abp.Web.Mvc.Authorization.AbpAuthorize]
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ActivationInfo()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Register(RegisterUserInput input)
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

            input.ProfileImage = ProfileImageHelper.GenerateRandomProfileImage();
            _userAppService.RegisterUser(input);

            return Json(new AbpMvcAjaxResponse { TargetUrl = Url.Action("ActivationInfo") });
        }

        public JsonResult SendPasswordResetLink(SendPasswordResetLinkInput input)
        {
            _userAppService.SendPasswordResetLink(input);

            return Json(new AbpMvcAjaxResponse());
        }

        [HttpGet]
        public ActionResult ResetPassword(int userId, string resetCode)
        {
            return View(new ResetPasswordViewModel {UserId = userId, ResetCode = resetCode});
        }

        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordInput input)
        {
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

            _userAppService.ResetPassword(input);

            return Json(new AbpMvcAjaxResponse { TargetUrl = Url.Action("Login") });
        }

        [Authorize]
        public JsonResult KeepSessionOpen()
        {
            return Json(new AbpMvcAjaxResponse());
        }
    }
}
