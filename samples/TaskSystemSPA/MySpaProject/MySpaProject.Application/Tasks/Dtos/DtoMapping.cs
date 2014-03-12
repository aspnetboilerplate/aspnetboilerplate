using AutoMapper;

namespace MySpaProject.Tasks.Dtos
{
    internal static class DtoMapping
    {
        public static void Map()
        {
            Mapper.CreateMap<Task, TaskDto>().ReverseMap();
        }
    }
}
