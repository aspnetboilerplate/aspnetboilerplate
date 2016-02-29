using Adorable.Application.Services.Dto;
using Adorable.AutoMapper;

namespace Adorable.TestBase.SampleApplication.People.Dto
{
    [AutoMap(typeof(Person))]
    public class PersonDto : EntityDto
    {
        public int ContactListId { get; set; }

        public string Name { get; set; }
    }
}