using System.Text;
using System.Web.Mvc;
using Abp.Web.Authentication;

namespace Abp.Web.Mvc.Controllers.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpAuthorizationController : AbpController
    {
        private readonly IAuthenticationScriptManager _authenticationScriptManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationScriptManager"></param>
        public AbpAuthorizationController(IAuthenticationScriptManager authenticationScriptManager)
        {
            _authenticationScriptManager = authenticationScriptManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ContentResult GetScripts()
        {
            var script = _authenticationScriptManager.GetAuthenticationScript();
            return Content(script, "application/x-javascript", Encoding.UTF8);
        }

    }
}
