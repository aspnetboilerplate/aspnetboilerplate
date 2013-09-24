using System.Collections.Generic;
using Abp.Services.Dto;

namespace Taskever.Services.Dto
{
    public class GetTasksOfUserOutput : IOutputDto
    {
        public IList<TaskDto> Tasks { get; set; }
    }
}