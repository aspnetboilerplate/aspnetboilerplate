using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    [AbpMvcAuthorize]
    public class AuthTest2Controller : AbpController
    {
        [AllowAnonymous]
        public ActionResult NonAuthorizedAction()
        {
            return Content("public content 2");
        }
        
        public ActionResult AuthorizedAction()
        {
            return Content("secret content 2");
        }
    }
}