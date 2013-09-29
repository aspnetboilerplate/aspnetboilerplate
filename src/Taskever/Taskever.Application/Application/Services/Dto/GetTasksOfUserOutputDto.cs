using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto
{
    public class GetTasksOfUserOutput : IOutputDto
    {
        public IList<TaskDto> Tasks { get; set; }
    }
}