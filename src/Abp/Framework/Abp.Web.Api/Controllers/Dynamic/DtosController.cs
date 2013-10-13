using System.Net.Http;
using System.Net.Http.Headers;
using Abp.WebApi.Controllers.Dynamic.Scripting.Dtos;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class DtosController : AbpApiController
    {
        public HttpResponseMessage Get()
        {
            //TODO: Caching script generation!
            var script = new DtoScriptBuilder().GenerateAll();
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }
}