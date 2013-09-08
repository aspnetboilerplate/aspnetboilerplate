using Taskever.Entities;

namespace Taskever.Services.Dto
{
    public static class DtoMapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Task, TaskDto>().ReverseMap();
        }
    }
}
