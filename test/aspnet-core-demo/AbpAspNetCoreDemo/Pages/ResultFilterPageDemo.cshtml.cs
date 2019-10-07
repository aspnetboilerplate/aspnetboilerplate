using Abp.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [IgnoreAntiforgeryToken]
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