using ExamCenter.Entities;
using ExamCenter.Services.Dto;

namespace ExamCenter.Web.Dependency
{
    public static class AutoMappingManager
    {
        public static void Map()
        {
            AutoMapper.Mapper.CreateMap<Question, QuestionDto>();
                //.ForMember(dm => dm.LastModifierUserId, expression => expression.MapFrom(d => d.LastModifier.Id));
        }
    }
}
