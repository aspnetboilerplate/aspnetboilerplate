using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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

        [Fact]
        public void Should_Retrieve_Soft_Deleteds_When_Filter_Disabled()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var ouw = uowManager.Begin())
            {
                uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

                var persons = _personRepository.GetAllList().OrderBy(p => p.Name).ToList();
                persons.Count.ShouldBe(2);
                persons[0].Name.ShouldBe("emre");
                persons[1].Name.ShouldBe("halil");

                ouw.Complete();
            }
        }
    }
}
