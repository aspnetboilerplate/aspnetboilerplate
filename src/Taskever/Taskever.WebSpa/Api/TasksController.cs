using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Data;
using Abp.Web.Controllers;
using AttributeRouting.Web.Http;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Taskever.Services;
using Taskever.Services.Dto;
using System.Threading;
using Abp.Services;
using System.Web;

namespace Taskever.Web.Api
{
    public class TasksController : AbpApiController, ITaskService
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        
        [GET("Mytasks")]
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
