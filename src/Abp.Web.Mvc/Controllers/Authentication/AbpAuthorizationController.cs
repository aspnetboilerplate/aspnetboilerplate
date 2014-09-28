using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Web.Authentication;

namespace Abp.Web.Mvc.Controllers.Authentication
{
    public class AbpAuthorizationController : AbpController
    {
        private readonly IAuthenticationScriptManager _authenticationScriptManager;

        public AbpAuthorizationController(IAuthenticationScriptManager authenticationScriptManager)
        {
            _authenticationScriptManager = authenticationScriptManager;
        }

        public ContentResult GetScripts()
        {
            var script = _authenticationScriptManager.GetAuthenticationScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }

    }
}
