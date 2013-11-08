using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Application.Services.Dto.Mappings
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<User, UserDto>()
                .ForMember(
                    user => user.ProfileImage,
                    configuration => configuration.ResolveUsing(
                        user => user.ProfileImage == null
                                    //TODO: How to implement this?
                                    ? "/Abp/Framework/images/user.png"
                                    : "/ProfileImages/" + user.ProfileImage
                                         )
                ).ReverseMap();

            AutoMapper.Mapper.CreateMap<RegisterUserInput, User>();
        }
    }
}
