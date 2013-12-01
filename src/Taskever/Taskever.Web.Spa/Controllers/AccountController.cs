using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpAccountController
    {
        public AccountController(IUserService userService)
            : base(userService)
        {

        }

        public override System.Web.Mvc.JsonResult Register(Abp.Modules.Core.Application.Services.Dto.RegisterUserInput registerUser)
        {
            registerUser.ProfileImage = ProfileImageHelper.GenerateRandomProfileImage();
            return base.Register(registerUser);
        }
    }

    public static class ProfileImageHelper
    {
        private static List<string> _preDefinedProfileImages;

        private static Random _rnd = new Random();

        static ProfileImageHelper()
        {
            _preDefinedProfileImages = new List<string>
                                           {
                                               "user_blue.png",
                                               "user_cyan.png",
                                               "user_green.png",
                                               "user_orange.png",
                                               "user_purple.png",
                                               "user_red.png",
                                               "user_yellow.png"
                                           };
        }

        public static string GenerateRandomProfileImage()
        {
            try
            {
                var imgIndex = _rnd.Next(0, _preDefinedProfileImages.Count);
                var imgFileName = _preDefinedProfileImages[imgIndex];
                var imgFilePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ProfileImages/predefined"), imgFileName);

                var userImageFileName = "0_" + _rnd.Next(0, int.MaxValue) + "_" + DateTime.Now.Ticks + Path.GetExtension(imgFilePath);
                var userImageFilePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ProfileImages"), userImageFileName);

                File.Copy(imgFilePath, userImageFilePath, true);

                return userImageFileName;
            }
            catch (Exception)
            {
                //TODO: Log?
                return null;
            }
        }
    }
}
