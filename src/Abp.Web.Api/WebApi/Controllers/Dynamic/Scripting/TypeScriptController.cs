using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Auditing;
using Abp.Web.Models;
using Abp.Web.Security;
using Abp.Web.Security.AntiForgery;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.TypeScript;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    [DontWrapResult]
    [DisableAuditing]
    [DisableAbpAntiForgeryTokenValidation]
    public class TypeScriptController : AbpApiController
    {
        readonly TypeScriptDefinitionGenerator _typeScriptDefinitionGenerator;
        readonly TypeScriptServiceGenerator _typeScriptServiceGenerator;
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
