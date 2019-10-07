using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    public class RazorPageAntiForgeryTokenTestModel : PageModel
    {
        [BindProperty] public string Message { get; set; }

        public void OnPost()
        {
            Message = "Post is done at " + DateTime.Now.ToShortTimeString();
        }
    }
}