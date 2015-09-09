using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace MyProject.People.Dtos
{
    public class GetAllPeopleOutput : IOutputDto
    {
        public List<PersonDto> People { get; set; }
    }
}