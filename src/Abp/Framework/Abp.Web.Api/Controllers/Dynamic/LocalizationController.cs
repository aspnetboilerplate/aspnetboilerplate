using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Localization.Sources;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

namespace Abp.WebApi.Controllers.Dynamic
{
    public class LocalizationController : AbpApiController
    {
        private readonly ILocalizationSourceManager _localizationSourceManager;

        public LocalizationController(ILocalizationSourceManager localizationSourceManager)
        {
            _localizationSourceManager = localizationSourceManager;
        }
        
        public HttpResponseMessage Get()
        {
            //TODO: Caching script generation!
            var script = new LocalizationScriptBuilder(_localizationSourceManager.GetAllSources()).BuildAll();

            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            response.Content.Headers.ContentEncoding.Add("utf-8");
            return response;
        }
    }
}