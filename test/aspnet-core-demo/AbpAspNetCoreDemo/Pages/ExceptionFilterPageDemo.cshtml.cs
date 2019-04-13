using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.RazorPages;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    public class ExceptionFilterPageDemoModel : AbpPageModel
    {
        public JsonResult OnGet()
        {
            throw new UserFriendlyException("OnGet");
        }

        public IActionResult OnPost()
        {
            throw new UserFriendlyException("OnPost");
        }
    }
}