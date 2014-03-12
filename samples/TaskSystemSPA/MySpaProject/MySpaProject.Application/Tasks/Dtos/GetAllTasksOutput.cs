using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace MySpaProject.Tasks.Dtos
{
    public class GetAllTasksOutput : IOutputDto
    {
        public List<TaskDto> Tasks { get; set; } 
    }
}