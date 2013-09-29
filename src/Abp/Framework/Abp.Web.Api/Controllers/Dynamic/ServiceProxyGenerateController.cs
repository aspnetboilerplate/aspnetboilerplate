using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Abp.Utils.Extensions;
using Abp.WebApi.Controllers.Dynamic.Scripting;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class ServiceProxiesController : AbpApiController
    {
        public HttpResponseMessage Get(string name)
        {
            var controllerInfo = DynamicApiControllerManager.FindServiceController(name.ToPascalCase());
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
