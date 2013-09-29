using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Authorization;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Security;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Models;
using Taskever.Web.Models;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginModel loginModel)
        {
            Thread.Sleep(5000);
            try
            {
                if (ModelState.IsValid)
                {
                    if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                    {
                        throw new UserFriendlyException("No user name or password!");
                    }

                    //FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);
                    var user = _userService.GetUserOrNull(loginModel.EmailAddress, loginModel.Password);
                    var identity = new AbpIdentity(user.Id, user.EmailAddress);
                    var authTicket = new FormsAuthenticationTicket(1, loginModel.EmailAddress, DateTime.Now, DateTime.Now.AddMinutes(15), false, identity.SerializeToString());
                    var encTicket = FormsAuthentication.Encrypt(authTicket);
                    var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    return Json(new AbpAjaxResult(true) { TargetUrl = "/" });
                }

                return Json(new AbpAjaxResult(new ErrorModel("Your form is invalid!")));
            }
            catch (UserFriendlyException ex)
            {
                //TODO: log ex as warning
                return Json(new AbpAjaxResult(new ErrorModel(ex.Message)));
            }
            catch (Exception ex)
            {
                //TODO: log ex as Error
                return Json(new AbpAjaxResult(new ErrorModel("System error!")));
            }
        }

        [Abp.Modules.Core.Authorization.AbpAuthorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult Register(RegisterUserInputDto registerUserDto)
        {
            _userService.RegisterUser(registerUserDto);
            return Login(new LoginModel { EmailAddress = registerUserDto.EmailAddress, Password = registerUserDto.Password });
        }
    }
}
