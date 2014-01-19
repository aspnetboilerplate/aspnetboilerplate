using System.Net.Http;
using System.Net.Http.Headers;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    /// <summary>
    /// Used to get localization javascript from clients.
    /// </summary>
    public class LocalizationController : AbpApiController
    {
        private readonly ILocalizationScriptManager _localizationScriptManager;

        public LocalizationController(ILocalizationScriptManager localizationScriptManager)
        {
            _localizationScriptManager = localizationScriptManager;
        }

        public HttpResponseMessage Get()
        {
            var script = _localizationScriptManager.GetLocalizationScript();

            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            response.Content.Headers.ContentEncoding.Add("utf-8");
            
            return response;
        }
    }
}