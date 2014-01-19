using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    public class GetTaskInput : IInputDto
    {
        public int Id { get; set; }
    }
}