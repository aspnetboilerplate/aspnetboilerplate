using System;
using System.Web.Mvc;
using Abp.Threading;

namespace MyProject.Web.Controllers
{
    public class NewsController : MyProjectControllerBase
    {
        public NewsController()
        {

        }


        // GET: News
        public ActionResult Index()
        {
            return View();
        }
    }
}