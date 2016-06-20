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
    }
}