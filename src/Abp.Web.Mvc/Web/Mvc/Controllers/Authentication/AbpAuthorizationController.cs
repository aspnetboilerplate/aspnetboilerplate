using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var script = _authorizationScriptManager.GetAuthenticationScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }

    }
}
