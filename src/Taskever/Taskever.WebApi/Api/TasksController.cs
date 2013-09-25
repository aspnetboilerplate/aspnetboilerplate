using System.Collections.Generic;
using Abp.WebApi.Authorization;
using Abp.WebApi.Controllers;
using Taskever.Application.Services.Dto;
using Taskever.Services;

namespace Taskever.Web.Api
{
    public class TasksController : AbpApiController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [AbpAuthorize]
        public IEnumerable<TaskDto> GetTasks()
        {
            return _taskService.GetMyTasks();
        }
    }
}
