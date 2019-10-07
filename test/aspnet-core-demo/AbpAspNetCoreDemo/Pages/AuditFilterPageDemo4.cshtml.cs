using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.RazorPages;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [DisableAuditing]
    [IgnoreAntiforgeryToken]
    public class AuditFilterPageDemo4Model : AbpPageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        public void OnPost()
        {
        }
    }
}