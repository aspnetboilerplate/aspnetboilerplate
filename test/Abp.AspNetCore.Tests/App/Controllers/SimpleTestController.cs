using System;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Models;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class SimpleTestController : AbpController
    {
        public ActionResult SimpleContent()
        {
            return Content("Hello world...");
        }

        public JsonResult SimpleJson()
        {
            return Json(new SimpleViewModel("Forty Two", 42));
        }

        public JsonResult SimpleJsonException(string message, bool userFriendly)
        {
            if (userFriendly)
            {
                throw new UserFriendlyException(message);
            }

            throw new Exception(message);
        }

        [DontWrapResult]
        public JsonResult SimpleJsonExceptionDownWrap()
        {
            throw new UserFriendlyException("an exception message");
        }

        [DontWrapResult]
        public JsonResult SimpleJsonDontWrap()
        {
            return Json(new SimpleViewModel("Forty Two", 42));
        }

        [HttpGet]
        [WrapResult]
        public void GetVoidTest()
        {
            
        }

        [DontWrapResult]
        public void GetVoidTestDontWrap()
        {

        }

        [HttpGet]
        public ActionResult GetActionResultTest()
        {
            return Content("GetActionResultTest-Result");
        }

        [HttpGet]
        public async Task<ActionResult> GetActionResultTestAsync()
        {
            await Task.Delay(0);
            return Content("GetActionResultTestAsync-Result");
        }

        [HttpGet]
        public async Task GetVoidExceptionTestAsync()
        {
            await Task.Delay(0);
            throw new UserFriendlyException("GetVoidExceptionTestAsync-Exception");
        }

        [HttpGet]
        public async Task<ActionResult> GetActionResultExceptionTestAsync()
        {
            await Task.Delay(0);
            throw new UserFriendlyException("GetActionResultExceptionTestAsync-Exception");
        }
    }
}
