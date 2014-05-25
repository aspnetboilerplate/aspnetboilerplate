using System.Net.Http;
using System.Net.Http.Headers;
using Abp.WebApi.Controllers.Dynamic.Formatters;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    /// <summary>
    /// This class is used to create proxies to call dynamic api methods from Javascript clients.
    /// </summary>
    public class AbpServiceProxiesController : AbpApiController
    {
        /// <summary>
        /// Gets javascript proxy for given service name.
        /// </summary>
        /// <param name="name">Name of the service</param>
        public HttpResponseMessage Get(string name)
        {
            var script = ScriptProxyManager.GetScript(name);
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }

        /// <summary>
        /// Gets javascript proxy for all services.
        /// </summary>
        public HttpResponseMessage GetAll()
        {
            var script = ScriptProxyManager.GetAllScript();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }
}