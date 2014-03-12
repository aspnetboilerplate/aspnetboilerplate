using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySpaProject.Tasks;

namespace MySpaProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaskAppService _taskAppService;

        public HomeController(ITaskAppService taskAppService)
        {
            _taskAppService = taskAppService;
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            var result = _taskAppService.GetAllTasks();

            return View();
        }
	}
}