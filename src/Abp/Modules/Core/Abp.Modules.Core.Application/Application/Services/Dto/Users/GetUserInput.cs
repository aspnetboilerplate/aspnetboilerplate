using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class GetUserInput : IInputDto
    {
        public int UserId { get; set; }
    }
}