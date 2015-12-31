using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Abp.Web.Models;
using Abp.WebApi.Controllers.Dynamic.Formatters;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    /// <summary>
    /// This class is used to create proxies to call dynamic api methods from Javascript clients.
    /// </summary>
    [Obsolete("Use AbpServiceProxiesController. This will be removed in later versions.")]
    [DontWrapResult]
    public class ServiceProxiesController : AbpApiController
    {
        private readonly ScriptProxyManager _scriptProxyManager;

        public ServiceProxiesController(ScriptProxyManager scriptProxyManager)
        {
            _scriptProxyManager = scriptProxyManager;
        }

        /// <summary>
        /// Gets javascript proxy for given service name.
        /// </summary>
        /// <param name="name">Name of the service</param>
        public HttpResponseMessage Get(string name)
        {
            var script = _scriptProxyManager.GetScript(name, ProxyScriptType.JQuery);
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }

        /// <summary>
        /// Gets javascript proxy for all services.
        /// </summary>
        public HttpResponseMessage GetAll()
        {
            var script = _scriptProxyManager.GetAllScript(ProxyScriptType.JQuery);
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }
}
