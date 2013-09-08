using System.Collections.Generic;
using Abp.Web.Controllers;
using Taskever.Services;
using Taskever.Services.Dto;

namespace Taskever.Web.Api
{
    public class TasksController : AbpApiController, ITaskService
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        
        public virtual IList<TaskDto> GetMyTasks()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            //Thread.Sleep(800);
            return _taskService.GetMyTasks(); //api/TaskService/GetMyTasks
        }

        public virtual void Create(TaskDto task)
        {
            //api/task/getAllTasks
            //api/task/create
            _taskService.Create(task);
            //return Request.CreateResponse(System.Net.HttpStatusCode.Created, task);
        }
    }
}
