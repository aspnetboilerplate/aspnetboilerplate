using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abp.Web.Mvc.Controllers;

namespace MySpaProject.Web.Controllers
{
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}