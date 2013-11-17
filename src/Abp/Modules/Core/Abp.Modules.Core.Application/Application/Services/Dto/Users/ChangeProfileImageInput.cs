using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class ChangeProfileImageInput :IInputDto
    {
        public string FileName { get; set; }
    }
}