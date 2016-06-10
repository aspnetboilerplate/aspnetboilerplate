using System.Linq;
using System.Threading.Tasks;
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

        [Fact]
        public void Test_Update()
        {
            var personInitial = UsingDbContext(context => context.People.Single(p => p.Name == "halil"));

            _personRepository.Update(new Person
            {
                Id = personInitial.Id,
                Name = "halil-updated",
                ContactListId = personInitial.ContactListId
            });

            var personFinal = UsingDbContext(context => context.People.Single(p => p.Id == personInitial.Id));
            personFinal.Name.ShouldBe("halil-updated");
        }
    }
}