using System.Net.Http;
using System.Net.Http.Headers;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class LocalizationController : AbpApiController
    {        
        public HttpResponseMessage Get()
        {
            var script = LocalizationScriptManager.GetScript();
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            response.Content.Headers.ContentEncoding.Add("utf-8");
            return response;
        }
    }
}