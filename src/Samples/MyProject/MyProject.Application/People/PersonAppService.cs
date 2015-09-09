using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using AutoMapper;
using MyProject.People.Dtos;

namespace MyProject.People
{
    public class PersonAppService : IPersonAppService //Optionally, you can derive from ApplicationService as we did for TaskAppService class.
    {
        private readonly IRepository<Person> _personRepository;

        //ABP provides that we can directly inject IRepository<Person> (without creating any repository class)
        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        //This method uses async pattern that is supported by ASP.NET Boilerplate
        public async Task<GetAllPeopleOutput> GetAllPeople()
        {
            return new GetAllPeopleOutput
                   {
                       People = Mapper.Map<List<PersonDto>>(await _personRepository.GetAllListAsync())
                   };
        }
    }
}