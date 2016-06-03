using Abp.Application.Services.Dto;

namespace Abp.TestBase.SampleApplication.People.Dto
{
    public class GetPeopleInput : IInputDto
    {
        public string NameFilter { get; set; }
    }
}