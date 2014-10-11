using System.Text;
using System.Web.Mvc;
using Abp.Web.Authorization;

namespace Abp.Web.Mvc.Controllers.Authentication
{
    public class AbpAuthorizationController : AbpController
    {
        private readonly IAuthorizationScriptManager _authorizationScriptManager;

        public AbpAuthorizationController(IAuthorizationScriptManager authorizationScriptManager)
        {
            _authorizationScriptManager = authorizationScriptManager;
        }

        public ContentResult GetScripts()
        {
            var script = _authorizationScriptManager.GetAuthorizationScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }
    }
}
