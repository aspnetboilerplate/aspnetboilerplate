using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Localization;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class LocalizationController : AbpApiController
    {
        private readonly ILocalizationManager _localizationManager;

        public LocalizationController(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
        }
        
        public HttpResponseMessage Get()
        {
            var script = new LocalizationScriptBuilder(_localizationManager.GetAllSources()).BuildAll();
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            response.Content.Headers.ContentEncoding.Add("utf-8");
            return response;
        }
    }
}