using Taskever.Application.Services.Dto.Activities;
using Taskever.Application.Services.Dto.Friendships;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Application.Services.Dto
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Task, TaskDto>().ReverseMap();
            AutoMapper.Mapper.CreateMap<Task, TaskWithAssignedUserDto>().ReverseMap();
            AutoMapper.Mapper.CreateMap<Friendship, FriendshipDto>().ReverseMap();

            AutoMapper.Mapper
                .CreateMap<Activity, ActivityDto>()
                .Include<CreateTaskActivity, CreateTaskActivityDto>()
                .Include<CompleteTaskActivity, CompleteTaskActivityDto>()
                .ReverseMap();
            AutoMapper.Mapper.CreateMap<CreateTaskActivity, CreateTaskActivityDto>();
            AutoMapper.Mapper.CreateMap<CompleteTaskActivity, CompleteTaskActivityDto>();
        }
    }
}
