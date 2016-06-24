using System;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;
using Abp.Web.Mvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    //TODO: Can be converted to a base class and move to Abp.AspNetCore package.
    public class ErrorController : AbpController
    {
        private readonly IErrorInfoBuilder _errorInfoBuilder;

        public ErrorController(IErrorInfoBuilder errorInfoBuilder)
        {
            _errorInfoBuilder = errorInfoBuilder;
        }

        public ActionResult Index()
        {
            var exHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var exception = exHandlerFeature != null
                                ? exHandlerFeature.Error
                                : new Exception("Unhandled exception!");

            return View(
                "Error",
                new ErrorViewModel(
                    _errorInfoBuilder.BuildForException(exception),
                    exception
                )
            );
        }
    }
}