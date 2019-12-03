using Abp.AspNetCore.Mvc.Controllers;
using Abp.Dependency;
using Abp.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ResultWrappingTestController : AbpController
    {
        public ResultWrappingTestController(IWebHostEnvironment environment)
        {
            
        }

        [HttpGet]
        [Route("ResultWrappingTest/Get")]
        public int Get()
        {
            return 42;
        }

        [HttpGet]
        [Route("ResultWrappingTest/GetDontWrap")]
        [DontWrapResult]
        public int GetDontWrap()
        {
            return 42;
        }
    }

    [DontWrapResult]
    public class ResultWrappingTest2Controller : Controller, ITransientDependency
    {
        public ResultWrappingTest2Controller(IWebHostEnvironment environment)
        {
            
        }

        [HttpGet]
        [Route("ResultWrappingTest2/Get")]
        public int Get()
        {
            return 43;
        }

        [HttpGet]
        [Route("ResultWrappingTest2/GetDontWrap")]
        //[DontWrapResult]
        public int GetDontWrap()
        {
            return 43;
        }
    }
}
