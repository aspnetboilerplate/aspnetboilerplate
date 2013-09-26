using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Taskever.Web.Controllers
{
    public class WidgetsController : Controller
    {
        public ActionResult LoginBox()
        {
            return PartialView("~/Widgets/LoginBox/LoginBox.cshtml");
        }
    }
}
