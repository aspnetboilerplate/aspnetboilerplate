namespace Abp.Users.Dto
{
    public static class UserDtosMapper
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
