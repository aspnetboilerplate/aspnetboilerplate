using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Dependency;
using Abp.Exceptions;
using Abp.Modules.Core.Mvc.Models;
using Abp.Security.Identity;
using Abp.Users;
using Abp.Users.Dto;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Taskever.Security.Identity;
using Taskever.Security.Users;
using Taskever.Users;
using Taskever.Web.Mvc.Models.Account;

namespace Taskever.Web.Mvc.Controllers
{
    public class AccountController : AbpController
    {
        private readonly IUserAppService _userAppService;

        private readonly TaskeverUserManager _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(IUserAppService userAppService, TaskeverUserManager userManager)
        {
            _userAppService = userAppService;
            _userManager = userManager;
        }

        public virtual ActionResult Login(string returnUrl = "/", string loginMessage = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginMessage = loginMessage;
            return View();
        }

        [HttpPost]
        public virtual async Task<JsonResult> Login(LoginModel loginModel, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                throw new AbpUserFriendlyException("Your form is invalid!");
            }

            var user = await _userManager.FindAsync(loginModel.EmailAddress, loginModel.Password);
            if (user == null)
            {
                throw new AbpUserFriendlyException("Invalid user name or password!");
            }
            
            await SignInAsync(user, loginModel.RememberMe);

            return Json(new AbpMvcAjaxResponse { TargetUrl = returnUrl });
        }

        private async Task SignInAsync(TaskeverUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
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

            if (!ModelState.IsValid)
            {
                throw new AbpUserFriendlyException("Your form is invalid!");
            }

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
            return View(new ResetPasswordViewModel { UserId = userId, ResetCode = resetCode });
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
