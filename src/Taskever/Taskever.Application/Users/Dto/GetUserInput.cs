using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class GetUserInput : IInputDto
    {
        public long UserId { get; set; }
    }
}