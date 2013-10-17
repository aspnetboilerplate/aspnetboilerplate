using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Abp.Utils.Extensions;
using Abp.WebApi.Controllers.Dynamic.Scripting.Proxy;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class ServiceProxiesController : AbpApiController
    {
        public HttpResponseMessage Get(string name)
        {
            if(!name.Contains("/"))
            {
                throw new ArgumentException("name is not valid");
            }

            //TODO: Refactor !!!
            var splitted = name.Split('/');
            var controllerInfo = DynamicApiControllerManager.FindServiceController(splitted[0].ToPascalCase(), splitted[1].ToPascalCase());
            if (controllerInfo == null)
            {
                throw new HttpException(404, "There is no such a service: " + name); //TODO: What to do if can not find?
            }

            //TODO: Caching script generation!
            var script = new ControllerScriptProxyGenerator().GenerateFor(controllerInfo);
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }
}
