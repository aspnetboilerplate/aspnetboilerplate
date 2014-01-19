using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class GetUserInput : IInputDto
    {
        public int UserId { get; set; }
    }
}