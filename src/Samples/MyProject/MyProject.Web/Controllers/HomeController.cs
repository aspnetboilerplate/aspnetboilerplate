using System;
using System.Web.Mvc;
using Abp.Threading;
using MyProject.People;
using MyProject.Tasks;
using MyProject.Tasks.Dtos;

namespace MyProject.Web.Controllers
{
    public class HomeController : MyProjectControllerBase
    {
        private readonly IPersonAppService _personAppService;

        private readonly ITaskAppService _taskAppService;

        public HomeController(IPersonAppService personAppService,
                              ITaskAppService taskAppService)
        {
            _personAppService = personAppService;
            _taskAppService = taskAppService;
        }

        public ActionResult Index()
        {

            // var output = _taskAppService.GetTasks(input);
            var output = _taskAppService.GetAll();            
            return View();
        }




        public ActionResult TaskList(GetTasksInput input)
        {
            var output = _taskAppService.GetTasks(input);

            return Request.IsAjaxRequest() ? View("_TaskItem", output) : View(output);
        }

        [HttpGet]
        public ActionResult NewTask()
        {
            var output = AsyncHelper.RunSync(() => _personAppService.GetAllPeople());

            return View(output);
        }

        [HttpPost]
        [ActionName("NewTask")]
        public ActionResult NewTaskPost(CreateTaskInput input)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("input Is Valid", input.GetType().FullName);
            }
            _taskAppService.CreateTask(input);

            return Json("OK");
        }

    }
}