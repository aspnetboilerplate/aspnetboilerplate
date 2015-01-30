using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.TestBase.SampleApplication.People.Dto;

namespace Abp.TestBase.SampleApplication.People
{
    public interface IPersonAppService : IApplicationService
    {
        ListResultOutput<PersonDto> GetPeople(GetPeopleInput input);

        void CreatePerson(CreatePersonInput input);
    }
}