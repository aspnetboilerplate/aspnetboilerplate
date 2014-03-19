using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySpaProject.Web.Controllers
{
    public class HomeController : MySpaProjectControllerBase
    {
        public ActionResult Index()
        { 
            return View();
        }
	}
}