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
    public class ResultFilterPageDemoModel : AbpPageModel
    {
        public IActionResult OnGet()
        {
            return Content("OnGet");
        }

        public JsonResult OnPost()
        {
            return new JsonResult("OnPost");
        }
    }
}