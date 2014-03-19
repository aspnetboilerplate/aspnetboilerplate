using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace MySpaProject.Tasks.Dtos
{
    public class GetTasksOutput : IOutputDto
    {
        public List<TaskDto> Tasks { get; set; } 
    }
}