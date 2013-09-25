using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Dto
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Task, TaskDto>().ReverseMap();
        }
    }
}
