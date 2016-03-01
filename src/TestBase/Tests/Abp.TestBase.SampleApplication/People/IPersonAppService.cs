using System.Threading.Tasks;
using Adorable.Application.Services;
using Adorable.Application.Services.Dto;
using Adorable.TestBase.SampleApplication.People.Dto;

namespace Adorable.TestBase.SampleApplication.People
{
    public interface IPersonAppService : IApplicationService
    {
        ListResultOutput<PersonDto> GetPeople(GetPeopleInput input);

        Task CreatePersonAsync(CreatePersonInput input);

        Task DeletePerson(EntityRequestInput input);

        string TestPrimitiveMethod(int a, string b, EntityRequestInput c);
    }
}