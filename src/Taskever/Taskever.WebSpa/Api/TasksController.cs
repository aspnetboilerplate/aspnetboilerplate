using System.Collections.Generic;
using Abp.Data;
using Abp.Web.Controllers;
using AttributeRouting.Web.Http;
using Castle.Core.Logging;
using Taskever.Services;
using Taskever.Services.Dto;

namespace Taskever.Web.Api
{
    public class TasksController : AbpApiController
    {
        private readonly ITaskService _questionService;

        public TasksController(ITaskService questionService)
        {
            _questionService = questionService;
        }

        [GET("Questionsss")]
        [UnitOfWork]
        public virtual IEnumerable<TaskDto> Get()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling
            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            return _questionService.GetAllTasks();
        }
    }
}
