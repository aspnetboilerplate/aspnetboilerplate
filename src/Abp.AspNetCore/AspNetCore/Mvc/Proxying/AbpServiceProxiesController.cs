using Abp.AspNetCore.Mvc.Controllers;
using Abp.Auditing;
using Abp.Web.Api.ProxyScripting;
using Abp.Web.Minifier;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Proxying
{
    [DontWrapResult]
    [DisableAuditing]
    public class AbpServiceProxiesController : AbpController
    {
        private readonly IApiProxyScriptManager _proxyScriptManager;
        private readonly IJavaScriptMinifier _javaScriptMinifier;

        public AbpServiceProxiesController(IApiProxyScriptManager proxyScriptManager, 
            IJavaScriptMinifier javaScriptMinifier)
        {
            _proxyScriptManager = proxyScriptManager;
            _javaScriptMinifier = javaScriptMinifier;
        }

        [Produces("application/x-javascript")]
        public ContentResult GetAll(ApiProxyGenerationModel model)
        {
            var script = _proxyScriptManager.GetScript(model.CreateOptions());
            return Content(model.Minify ? _javaScriptMinifier.Minify(script) : script, "application/x-javascript");
        }
    }
}
