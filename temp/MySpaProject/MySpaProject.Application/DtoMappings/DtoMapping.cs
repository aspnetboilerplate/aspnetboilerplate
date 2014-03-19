using AutoMapper;
using MySpaProject.People;
using MySpaProject.People.Dtos;
using MySpaProject.Tasks;
using MySpaProject.Tasks.Dtos;

namespace MySpaProject.DtoMappings
{
    internal static class DtoMapping
    {
        public static void Map()
        {
            Mapper.CreateMap<Task, TaskDto>();
            Mapper.CreateMap<Person, PersonDto>();
        }
    }
}
