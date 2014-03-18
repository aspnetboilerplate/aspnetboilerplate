using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Abp.Utils.Helpers;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Scripting.Localization;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    /// <summary>
    /// Used to get localization javascript from clients.
    /// </summary>
    public class AbpLocalizationController : AbpApiController
    {
        private readonly ILocalizationScriptManager _localizationScriptManager;

        public AbpLocalizationController(ILocalizationScriptManager localizationScriptManager)
        {
            _localizationScriptManager = localizationScriptManager;
        }

        public HttpResponseMessage Get()
        {
            var script = _localizationScriptManager.GetLocalizationScript();
            var response = Request.CreateResponse(HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            response.Content.Headers.ContentEncoding.Add("utf-8");
            return response;
        }

        [HttpPost]
        public HttpResponseMessage ChangeLanguage(string lang)
        {
            if (!GlobalizationHelper.IsValidCultureCode(lang))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var response = new HttpResponseMessage();
            var cookie = new CookieHeaderValue("Abp.Localization.Language", lang)
                         {
                             Expires = DateTimeOffset.Now.AddYears(2),
                             Domain = Request.RequestUri.Host,
                             Path = "/"
                         };

            response.Headers.AddCookies(new[] { cookie });
            return response;
        }
    }
}