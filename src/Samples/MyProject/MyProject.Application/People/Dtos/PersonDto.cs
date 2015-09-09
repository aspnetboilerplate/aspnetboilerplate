using Abp.Application.Services.Dto;

namespace MyProject.People.Dtos
{
    public class PersonDto : EntityDto
    {
        public string Name { get; set; }
    }
}