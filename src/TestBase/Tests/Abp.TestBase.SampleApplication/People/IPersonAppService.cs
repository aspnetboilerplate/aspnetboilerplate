using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.TestBase.SampleApplication.People.Dto;

namespace Abp.TestBase.SampleApplication.People
{
    public interface IPersonAppService : IApplicationService
    {
        ListResultOutput<PersonDto> GetPeople(GetPeopleInput input);

        Task CreatePersonAsync(CreatePersonInput input);

        Task DeletePerson(EntityRequestInput input);

        string TestPrimitiveMethod(int a, string b, EntityRequestInput c);
    }
}