using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Data;
using Abp.Web.Controllers;
using AttributeRouting.Web.Http;
using Castle.Core.Logging;
using Taskever.Services;
using Taskever.Services.Dto;
using System.Threading;

namespace Taskever.Web.Api
{
    #region Test purposes 

    public class TestSelector : IHttpControllerSelector
    {
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            return null;            
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return null;
        }
    }

    public class TaskServiceController : AbpServiceApiController<ITaskService>
    {
        
    }

    public class AbpServiceApiController<T>
    {

    }

    #endregion

    public class TasksController : AbpApiController
    {
        private readonly ITaskService _questionService;

        public TasksController(ITaskService questionService)
        {
            _questionService = questionService;
        }

        [GET("Mytasks")]
        public virtual IEnumerable<TaskDto> Get()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            //Thread.Sleep(800);
            return _questionService.GetMyTasks(); //api/TaskService/GetMyTasks
        }

        public virtual HttpResponseMessage Post(TaskDto task)
        {
            //api/task/getAllTasks
            //api/task/create
            _questionService.Create(task);
            return Request.CreateResponse(System.Net.HttpStatusCode.Created, task);
        }
    }
}
