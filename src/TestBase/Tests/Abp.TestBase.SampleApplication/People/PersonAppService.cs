using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.TestBase.SampleApplication.People.Dto;

namespace Abp.TestBase.SampleApplication.People
{
    public class PersonAppService : ApplicationService, IPersonAppService
    {
        private readonly IRepository<Person> _personRepository;

        public PersonAppService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public ListResultOutput<PersonDto> GetPeople(GetPeopleInput input)
        {
            var query = _personRepository.GetAll();

            if (!input.NameFilter.IsNullOrEmpty())
            {
                query = query.Where(p => p.Name.Contains(input.NameFilter));
            }

            return new ListResultOutput<PersonDto>(query.ToList().Select(p => new PersonDto {Id = p.Id, Name = p.Name}).ToList());
        }

        public void CreatePerson(CreatePersonInput input)
        {
            var person = new Person {Name = input.Name};
            _personRepository.Insert(person);
        }
    }
}
