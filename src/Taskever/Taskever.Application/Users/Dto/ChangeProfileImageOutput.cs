using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class ChangeProfileImageOutput:IOutputDto
    {
        public string OldFileName { get; set; }
    }
}