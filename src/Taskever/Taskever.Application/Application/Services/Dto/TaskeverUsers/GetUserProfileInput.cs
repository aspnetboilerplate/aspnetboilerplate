using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.TaskeverUsers
{
    public class GetUserProfileInput : IInputDto
    {
        public int UserId { get; set; }
    }
}