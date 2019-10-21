using Abp.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [IgnoreAntiforgeryToken]
    public class AuditFilterPageDemo5Model : AbpPageModel
    {
        public JsonResult OnPostJson()
        {
            return new JsonResult(new {StrValue = "Forty Two", IntValue = 42});
        }

        public ObjectResult OnPostObject()
        {
            return new ObjectResult(new { StrValue = "Forty Two", IntValue = 42 });
        }

        public IActionResult OnPostString()
        {
            return Content("test");
        }
    }
}