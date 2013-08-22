using System.Web.Mvc;
using Abp.Web.Controllers;

namespace ExamCenter.Web.Controllers
{
    public class HomeController : AbpController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
