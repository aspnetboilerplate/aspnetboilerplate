using System.IO;
using System.Web;
using System.Web.Mvc;
using Abp.Modules.Core.Data.Repositories;
using Abp.Web.Models;
using Abp.Web.Mvc.Authorization;

namespace Taskever.Web.Controllers
{
    [AbpAuthorize]
    public class ProfileImageController : TaskeverController
    {
        private readonly IUserRepository _userRepository;

        public ProfileImageController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public JsonResult UploadTempProfileImage()
        {
            if (Request.Files.Count > 0)
            {
                var uploadfile = Request.Files[0];

                var ext = Path.GetExtension(uploadfile.FileName);
                var path = GenerateTempProfileImagePath(ext);
                if (System.IO.File.Exists(path)) //TODO: Make a helper method for that! DeleteFileIfExists
                {
                    System.IO.File.Delete(path);
                }

                uploadfile.SaveAs(path);
            }

            return Json(new AbpAjaxResponse(true));
        }

        //[HttpPost]
        public JsonResult CancelTempProfileImage()
        {
            var path = GenerateTempProfileImagePath(".jpg");
            if (System.IO.File.Exists(path)) //TODO: Make a helper method for that! DeleteFileIfExists
            {
                System.IO.File.Delete(path);
            }

            return Json(new AbpAjaxResponse(true));
        }

        public JsonResult RemoveTempProfileImage()
        {
            var path = GenerateTempProfileImagePath(".jpg");
            if (System.IO.File.Exists(path)) //TODO: Make a helper method for that! DeleteFileIfExists
            {
                System.IO.File.Delete(path);
            }

            return Json(new AbpAjaxResponse(true));
        }

        public JsonResult AcceptTempProfileImage()
        {
            var tempPath = GenerateTempProfileImagePath(".jpg");
            var realPath = GenerateProfileImagePath(".jpg");

            System.IO.File.Copy(tempPath, realPath, true);

            var fileName = Path.GetFileName(realPath);

            var userId = Abp.Modules.Core.Domain.Entities.User.CurrentUserId;
            var user = _userRepository.Get(userId);
            user.ProfileImage = fileName;
            _userRepository.Update(user);

            return Json(new AbpAjaxResponse(true));
        }

        private string GenerateProfileImagePath(string ext)
        {
            var userId = Abp.Modules.Core.Domain.Entities.User.CurrentUserId;
            return Path.Combine(Server.MapPath("~/ProfileImages"), userId + ext);
        }

        private string GenerateTempProfileImagePath(string ext)
        {
            var userId = Abp.Modules.Core.Domain.Entities.User.CurrentUserId;
            return Path.Combine(Server.MapPath("~/ProfileImages/Temp"), userId + ext);
        }
    }
}