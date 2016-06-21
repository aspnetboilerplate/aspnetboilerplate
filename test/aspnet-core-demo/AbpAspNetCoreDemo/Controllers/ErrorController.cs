using System;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Mvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ErrorController : AbpController
    {
        public ActionResult Index()
        {
            var exHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exHandlerFeature == null)
            {
                return View("Error", new ErrorViewModel(new Exception("Unhandled exception!")));
            }

            return View("Error", new ErrorViewModel(exHandlerFeature.Error));
        }
    }
}