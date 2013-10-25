using Taskever.Application.Services.Dto.Friendships;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Dto
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Task, TaskDto>().ReverseMap();
            AutoMapper.Mapper.CreateMap<Friendship, FriendshipDto>().ReverseMap();
        }
    }
}
