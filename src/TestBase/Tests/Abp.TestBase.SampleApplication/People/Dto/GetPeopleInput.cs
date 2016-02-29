using Adorable.Application.Services.Dto;

namespace Adorable.TestBase.SampleApplication.People.Dto
{
    public class GetPeopleInput : IInputDto
    {
        public string NameFilter { get; set; }
    }
}