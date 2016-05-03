using Abp.Web.Models;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    [DontWrapResult]
    public class TypeScriptController : AbpApiController
    {
        private readonly TypeScriptDefinitionGenerator _typeScriptDefinitionGenerator;
        private readonly TypeScriptServiceGenerator _typeScriptServiceGenerator;

        public TypeScriptController(TypeScriptDefinitionGenerator typeScriptDefinitionGenerator, TypeScriptServiceGenerator typeScriptServiceGenerator)
        {
            _typeScriptDefinitionGenerator = typeScriptDefinitionGenerator;
            _typeScriptServiceGenerator = typeScriptServiceGenerator;
        }

        public HttpResponseMessage Get(bool isCompleteService = false)
        {
            if (isCompleteService)
            {
                var script = _typeScriptServiceGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
            else
            {
                var script = _typeScriptDefinitionGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
        }
    }
}