using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
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

            var people = query.ToList();

            return new ListResultOutput<PersonDto>(people.MapTo<List<PersonDto>>());
        }

        public void CreatePerson(CreatePersonInput input)
        {
            _personRepository.Insert(input.MapTo<Person>());
        }
    }
}
