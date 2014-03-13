using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abp.Web.Mvc.Controllers;
using MySpaProject.Tasks;

namespace MySpaProject.Web.Controllers
{
    public class HomeController : AbpController
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