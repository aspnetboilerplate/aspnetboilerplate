using System.Linq;
using Abp.Domain.Repositories;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonRepository_General_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public PersonRepository_General_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void Should_Delete_Entity_Not_In_Context()
        {
            var person = UsingDbContext(context => context.People.Single(p => p.Name == "halil"));
            _personRepository.Delete(person);
            UsingDbContext(context => context.People.FirstOrDefault(p => p.Name == "halil")).IsDeleted.ShouldBe(true);
        }
    }
}