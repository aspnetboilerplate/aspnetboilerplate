using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class WrapResultTestController : AbpController
    {
        [HttpGet]
        [Route("WrapResultTest/Get")]
        public int Get()
        {
            return 42;
        }

        [HttpGet]
        [Route("WrapResultTest/GetDontWrap")]
        [DontWrapResult]
        public int GetDontWrap()
        {
            return 42;
        }
    }
}
