using System.IO;
using System.Web.Mvc;
using Abp.Modules.Core.Data.Repositories;
using Abp.Utils.Helpers;
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
        public JsonResult UploadProfileImage()
        {
            if (Request.Files.Count > 0)
            {
                var uploadfile = Request.Files[0];
                if (uploadfile != null)
                {
                    var path = GenerateProfileImagePath(Path.GetExtension(uploadfile.FileName));
                    FileHelper.DeleteIfExists(path);
                    uploadfile.SaveAs(path);

                    var fileName = Path.GetFileName(path);

                    var currentUser = _userRepository.Get(Abp.Modules.Core.Domain.Entities.User.CurrentUserId);
                    currentUser.ProfileImage = fileName;
                    _userRepository.Update(currentUser);

                    return Json(new AbpAjaxResponse(new
                                                        {
                                                            imageUrl = "/ProfileImages/" + fileName
                                                        }));
                }
            }

            return Json(new AbpAjaxResponse(false)); //TODO: Error message?
        }

        private string GenerateProfileImagePath(string ext)
        {
            var userId = Abp.Modules.Core.Domain.Entities.User.CurrentUserId;
            return Path.Combine(Server.MapPath("~/ProfileImages"), userId + ext);
        }
    }
}