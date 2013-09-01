using Taskever.Entities;
using Taskever.Services.Dto;

namespace Taskever.Web.Dependency
{
    public static class AutoMappingManager
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Task, TaskDto>();
                //.ForMember(dm => dm.LastModifierUserId, expression => expression.MapFrom(d => d.LastModifier.Id));
        }
    }
}
