using System;
using System.IO;
using System.Web.Mvc;
using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Utils.Helpers;
using Abp.Web.Models;
using Abp.Web.Mvc.Authorization;

namespace Taskever.Web.Controllers
{
    [AbpAuthorize]
    public class ProfileImageController : TaskeverController
    {
        private readonly IUserService _userService;

        public ProfileImageController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public JsonResult UploadProfileImage()
        {
            if (Request.Files.Count > 0)
            {
                var uploadfile = Request.Files[0];
                if (uploadfile != null)
                {
                    //Save uploaded file
                    var tempPath = GenerateProfileImagePath(uploadfile.FileName);
                    FileHelper.DeleteIfExists(tempPath);
                    uploadfile.SaveAs(tempPath);

                    //Change profile picture
                    var fileName = Path.GetFileName(tempPath);
                    var result = _userService.ChangeProfileImage(new ChangeProfileImageInput { FileName = fileName });
                    
                    //Delete old file
                    var oldFilePath = Path.Combine(Server.MapPath("~/ProfileImages"), result.OldFileName);
                    FileHelper.DeleteIfExists(oldFilePath);

                    //Return response
                    return Json(new AbpAjaxResponse(new
                                                        {
                                                            imageUrl = "/ProfileImages/" + fileName
                                                        }));
                }
            }

            //No file
            return Json(new AbpAjaxResponse(false)); //TODO: Error message?
        }

        private string GenerateProfileImagePath(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            var userId = Abp.Modules.Core.Domain.Entities.User.CurrentUserId;
            return Path.Combine(Server.MapPath("~/ProfileImages"), userId + "_" + DateTime.Now.Ticks + ext);
        }
    }
}