using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.People.Dto
{
    [AutoMap(typeof(Person))]
    public class PersonDto : EntityDto
    {
        public string Name { get; set; }
    }
}