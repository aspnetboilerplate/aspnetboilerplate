using Abp.AspNetCore.Mvc.Controllers;
using Abp.Auditing;
using Abp.Web.Api.ProxyScripting;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Proxying
{
    [DontWrapResult]
    [DisableAuditing]
    public class AbpServiceProxiesController : AbpController
    {
        private readonly IApiProxyScriptManager _proxyScriptManager;

        public AbpServiceProxiesController(IApiProxyScriptManager proxyScriptManager)
        {
            _proxyScriptManager = proxyScriptManager;
        }

        [Produces("application/x-javascript")]
        public ContentResult GetAll(ApiProxyGenerationModel model)
        {
            var script = _proxyScriptManager.GetScript(model.CreateOptions());
            return Content(script, "application/x-javascript");
        }
    }
}
