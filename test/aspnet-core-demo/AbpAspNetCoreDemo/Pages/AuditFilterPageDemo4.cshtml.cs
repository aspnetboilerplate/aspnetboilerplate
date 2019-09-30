using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
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