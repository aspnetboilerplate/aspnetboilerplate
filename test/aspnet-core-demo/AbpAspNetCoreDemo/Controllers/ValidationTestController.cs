using AbpAspNetCoreDemo.Models.ValidationTest;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ValidationTestController : MyDemoControllerBase
    {
        public ActionResult Index(IndexViewModel viewModel)
        {
            ViewBag.Message = L("HelloWorld");

            return View(viewModel);
        }
    }
}