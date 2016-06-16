using Abp.AspNetCore.Mvc.Controllers;
using AbpAspNetCoreDemo.Models.ValidationTest;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ValidationTestController : AbpController
    {
        public ActionResult Index(IndexViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}