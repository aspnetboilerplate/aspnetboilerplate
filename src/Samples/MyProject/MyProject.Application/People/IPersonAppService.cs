using System.Threading.Tasks;
using Abp.Application.Services;
using MyProject.People.Dtos;

namespace MyProject.People
{
    public interface IPersonAppService : IApplicationService
    {
        Task<GetAllPeopleOutput> GetAllPeople();
    }
}
