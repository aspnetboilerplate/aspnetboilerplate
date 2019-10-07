using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    public class RazorPageAntiforgeryTokenTestModel : PageModel
    {
        [BindProperty] public string Message { get; set; }

        public void OnPost()
        {
            Message = "Post is done at " + DateTime.Now.ToShortTimeString();
        }
    }
}