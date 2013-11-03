using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTaskInput : IInputDto
    {
        public int Id { get; set; }
    }
}