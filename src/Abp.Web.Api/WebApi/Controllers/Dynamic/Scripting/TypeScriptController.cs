using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Web.Models;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    [DontWrapResult]    
    public class TypeScriptController : AbpApiController
    {
        readonly TypeScriptDefinitionGenerator _typeScriptDefinitionGenerator;
        public TypeScriptController(TypeScriptDefinitionGenerator typeScriptDefinitionGenerator)
        {
            _typeScriptDefinitionGenerator = typeScriptDefinitionGenerator;
        }
        public HttpResponseMessage Get()
        {
            var script = _typeScriptDefinitionGenerator.GetScript();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }
}
