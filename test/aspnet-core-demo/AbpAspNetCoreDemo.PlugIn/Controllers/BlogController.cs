using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.PlugIn.Controllers;

public class BlogController : AbpController
{
    public ActionResult Index()
    {
        return View();
    }
}