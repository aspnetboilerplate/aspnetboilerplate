using System;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ExceptionTestController : MyDemoControllerBase
    {
        public ActionResult ViewEx()
        {
            throw new Exception("This is a regular exception from an action returns ActionResult");
        }

        public ActionResult ViewExUserFriendly()
        {
            throw new UserFriendlyException("This is a user friendly exception from an action returns ActionResult");
        }

        public JsonResult JsonExUserFriendly()
        {
            throw new UserFriendlyException("Test User Friendly Exception", "This is a user friendly exception directly shown to the user.");
        }
    }
}