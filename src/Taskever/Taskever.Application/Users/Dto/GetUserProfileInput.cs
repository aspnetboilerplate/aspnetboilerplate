using Abp.Application.Services.Dto;

namespace Taskever.Users.Dto
{
    public class GetUserProfileInput : IInputDto
    {
        public long UserId { get; set; }
    }
}