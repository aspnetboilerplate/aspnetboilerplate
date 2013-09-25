using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Application.Services.Dto.Mappings
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>();
        }
    }
}
