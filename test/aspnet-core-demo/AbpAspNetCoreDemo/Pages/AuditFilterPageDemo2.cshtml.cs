using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    [DisableAuditing]
    [IgnoreAntiforgeryToken]
    public class AuditFilterPageDemo2Model : PageModel
    {
        public void OnGet()
        {
            
        }

        public void OnPost()
        {

        }
    }
}