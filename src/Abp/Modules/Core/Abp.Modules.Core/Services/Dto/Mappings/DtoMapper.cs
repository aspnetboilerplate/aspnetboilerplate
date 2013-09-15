using Abp.Modules.Core.Entities;

namespace Abp.Modules.Core.Services.Dto.Mappings
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>();
        }
    }
}
