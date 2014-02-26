using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class ChangeProfileImageInput :IInputDto
    {
        public string FileName { get; set; }
    }
}