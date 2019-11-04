using System;
using System.Globalization;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Models;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Timing;
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

        public ObjectResult SimpleObject()
        {
            return new ObjectResult(new SimpleViewModel("Forty Two", 42));
        }

        public string SimpleString()
        {
            return "test";
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
        public async Task<ActionResult> GetActionResultTest2()
        {
            await Task.Delay(0);
            return Content("GetActionResultTestAsync-Result");
        }

        [HttpGet]
        public async Task GetVoidExceptionTest()
        {
            await Task.Delay(0);
            throw new UserFriendlyException("GetVoidExceptionTestAsync-Exception");
        }

        [HttpGet]
        public async Task<ActionResult> GetActionResultExceptionTest()
        {
            await Task.Delay(0);
            throw new UserFriendlyException("GetActionResultExceptionTestAsync-Exception");
        }

        [HttpGet]
        public ActionResult GetCurrentCultureNameTest()
        {
            return Content(CultureInfo.CurrentCulture.Name);
        }

        [HttpGet]
        public string GetDateTimeKind(SimpleDateModel input)
        {
            return input.Date.Kind.ToString().ToLower();
        }

        [HttpGet]
        public string GetNotNormalizedDateTimeKindProperty(SimpleDateModel2 input)
        {
            return input.Date.Kind.ToString();
        }


        [HttpGet]
        public SimpleDateModel2 GetNotNormalizedDateTimeKindProperty2(string date)
        {
            return new SimpleDateModel2
            {
                Date = Convert.ToDateTime(date)
            };
        }

        [HttpGet]
        public SimpleDateModel3 GetNotNormalizedDateTimeKindProperty3(string date)
        {
            return new SimpleDateModel3
            {
                Date = Convert.ToDateTime(date)
            };
        }

        [HttpGet]
        public SimpleDateModel4 GetNotNormalizedDateTimeKindProperty4([DisableDateTimeNormalization]DateTime date)
        {
            return new SimpleDateModel4
            {
                Date = date
            };
        }

        [HttpGet]
        public string GetNotNormalizedDateTimeKindClass(SimpleDateModel3 input)
        {
            return input.Date.Kind.ToString().ToLower();
        }
    }
}
