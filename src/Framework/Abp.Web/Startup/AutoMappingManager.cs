using Abp.Entities.Core;
using Abp.Services.Core.Dto;

namespace Abp.Web.Startup
{
    public static class AutoMappingManager
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>();
        }
    }
}
