using Abp.Domain.Repositories;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonRepository_SoftDelete_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public PersonRepository_SoftDelete_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();

            UsingDbContext(context => context.People.Add(new Person() { Name = "emre" }));
            UsingDbContext(context => context.People.Add(new Person() { Name = "halil", IsDeleted = true}));
        }

        [Fact]
        public void Should_Not_Retrieve_Soft_Deleteds()
        {
            var persons = _personRepository.GetAllList();
            persons.Count.ShouldBe(1);
            persons[0].Name.ShouldBe("emre");
        }
    }
}
