using Abp.WebApi.Controllers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Abp.Web.Models;
using System.Net.Http;
using System.Text;

namespace Abp.Web.Api.Swagger.Controller
{
    [DontWrapResult]
    public class SwaggerInfoController : AbpApiController
    {    
        
        [HttpGet]
        public HttpResponseMessage Get()
        {            
            return new HttpResponseMessage() { Content = new StringContent(SwaggerBuilder.Json, Encoding.UTF8, "application/json") };
        }
    }


}
