using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTasksOutput : IOutputDto
    {
        public IList<TaskDto> Tasks { get; set; }
    }
}