using System;
using System.IO;
using System.Web.Mvc;
using Abp.Users;
using Abp.Users.Dto;
using Abp.Utils.Helpers.IO;
using Abp.Web.Models;
using Abp.Web.Mvc.Authorization;

namespace Taskever.Web.Mvc.Controllers
{
    [AbpAuthorize]
    public class ProfileImageController : TaskeverController
    {
        private readonly IUserAppService _userAppService;

        public ProfileImageController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
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
                    var tempPath = GenerateProfileImagePath(Path.GetExtension(uploadfile.FileName));
                    FileHelper.DeleteIfExists(tempPath);
                    uploadfile.SaveAs(tempPath);

                    //Change profile picture
                    var fileName = Path.GetFileName(tempPath);
                    var result = _userAppService.ChangeProfileImage(new ChangeProfileImageInput { FileName = fileName });
                    
                    //Delete old file
                    if(!string.IsNullOrWhiteSpace(result.OldFileName))
                    {
                        var oldFilePath = Path.Combine(Server.MapPath("~/ProfileImages"), result.OldFileName);
                        FileHelper.DeleteIfExists(oldFilePath);
                    }

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

        private string GenerateProfileImagePath(string fileExtension)
        {
            var userId = Abp.Users.User.CurrentUserId;
            return Path.Combine(Server.MapPath("~/ProfileImages"), userId + "_" + DateTime.Now.Ticks + fileExtension);
        }
    }
}