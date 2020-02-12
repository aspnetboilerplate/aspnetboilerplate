using Abp.AspNetCore.Mvc.RazorPages;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [IgnoreAntiforgeryToken]
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