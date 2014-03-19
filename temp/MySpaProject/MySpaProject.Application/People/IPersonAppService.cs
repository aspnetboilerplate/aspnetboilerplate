using Abp.Application.Services;
using MySpaProject.People.Dtos;

namespace MySpaProject.People
{
    public interface IPersonAppService : IApplicationService
    {
        GetAllPeopleOutput GetAllPeople();
    }
}
