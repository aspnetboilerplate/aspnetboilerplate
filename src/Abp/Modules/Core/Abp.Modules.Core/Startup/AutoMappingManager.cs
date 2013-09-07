using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services.Dto;

namespace Abp.Modules.Core.Startup
{
    public static class AutoMappingManager
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>(); //TODO: Move to Abp.Modules.Core!
        }
    }
}
