using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class ChangeProfileImageOutput:IOutputDto
    {
        public string OldFileName { get; set; }
    }
}