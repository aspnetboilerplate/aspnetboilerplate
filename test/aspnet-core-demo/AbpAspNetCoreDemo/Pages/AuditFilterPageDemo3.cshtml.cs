using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    [IgnoreAntiforgeryToken]
    public class AuditFilterPageDemo3Model : PageModel
    {
        [DisableAuditing]
        public void OnGet()
        {

        }

        [DisableAuditing]
        public void OnPost()
        {

        }
    }
}