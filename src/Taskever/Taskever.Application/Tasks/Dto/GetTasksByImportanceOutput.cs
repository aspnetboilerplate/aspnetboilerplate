using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    public class GetTasksByImportanceOutput : IOutputDto
    {
        public IList<TaskDto> Tasks { get; set; }
    }
}