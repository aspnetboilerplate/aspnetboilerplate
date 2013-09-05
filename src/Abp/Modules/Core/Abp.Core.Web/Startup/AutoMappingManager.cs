using Abp.Modules.Core.Entities.Core;
using Abp.Modules.Core.Services.Dto;

namespace Abp.Startup
{
    public static class AutoMappingManager
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>();
        }
    }
}
