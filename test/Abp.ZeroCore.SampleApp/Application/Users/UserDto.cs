using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.ZeroCore.SampleApp.Core;

namespace Abp.ZeroCore.SampleApp.Application.Users
{
    [AutoMap(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public string UserName { get; set; }
    }
}