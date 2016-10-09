using System;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class NameConflictController : AbpController
    {
        public string GetSelfActionUrl()
        {
            return Url.Action("GetSelfActionUrl", "NameConflict");
        }

        public string GetAppServiceActionUrlWithArea()
        {
            //Gets URL of NameConflictAppService.GetConstantString action
            return Url.Action("GetConstantString", "NameConflict", new { area = "app"});
        }
    }
}
