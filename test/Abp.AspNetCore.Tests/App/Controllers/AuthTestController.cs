using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.Controllers
{
    public class AuthTestController : AbpController
    {
        public ActionResult NonAuthorizedAction()
        {
            return Content("public content");
        }

        [Authorize]
        public ActionResult AuthorizedAction()
        {
            return Content("secret content");
        }

        [AbpMvcAuthorize]
        public ActionResult AbpMvcAuthorizedAction()
        {
            return Content("secret content");
        }

        [AbpMvcAuthorize]
        public AjaxResponse AbpMvcAuthorizedActionReturnsObject()
        {
            return new AjaxResponse("OK");
        }
    }
}
