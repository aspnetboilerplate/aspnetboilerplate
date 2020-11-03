using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class ResponseCacheTestController : AbpController
    {
        [HttpGet]
        [Route("ResponseCacheTest/Get")]
        public int Get()
        {
            return 42;
        }

        [HttpGet]
        [Route("ResponseCacheTest/GetWithCache")]
        [ResponseCache(Duration = 60)]
        public int GetWithCache()
        {
            return 42;
        }

        [HttpGet]
        [Route("ResponseCacheTest/GetWithoutCache")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public int GetWithoutCache()
        {
            return 42;
        }
    }
}
