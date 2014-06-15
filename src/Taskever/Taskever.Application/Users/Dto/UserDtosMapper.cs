using Abp.Security.Users;
using Taskever.Security.Users;

namespace Abp.Users.Dto
{
    public static class UserDtosMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<TaskeverUser, UserDto>()
                .ForMember(
                    user => user.ProfileImage,
                    configuration => configuration.ResolveUsing(
                        user => user.ProfileImage == null
                                    //TODO: How to implement this?
                                    ? ""
                                    : "ProfileImages/" + user.ProfileImage
                                         )
                ).ReverseMap();

            AutoMapper.Mapper.CreateMap<RegisterUserInput, TaskeverUser>();

            AutoMapper.Mapper.CreateMap<AbpUser, UserDto>().ReverseMap();

            AutoMapper.Mapper.CreateMap<RegisterUserInput, AbpUser>();
        }
    }
}
