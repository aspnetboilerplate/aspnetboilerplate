using System.Linq;
using Abp.Domain.Repositories;
using NHibernate.Linq;
using Shouldly;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class Basic_Repository_Tests : NHibernateTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public Basic_Repository_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            UsingSession(session => session.Save(new Person() {Name = "emre"}));
        }

        [Fact]
        public void Should_Get_All_People()
        {
            _personRepository.GetAllList().Count.ShouldBe(1);
        }

        [Fact]
        public void Should_Insert_People()
        {
            _personRepository.Insert(new Person() {Name = "halil"});

            var insertedPerson = UsingSession(session => session.Query<Person>().FirstOrDefault(p => p.Name == "halil"));
            insertedPerson.ShouldNotBe(null);
            insertedPerson.IsTransient().ShouldBe(false);
            insertedPerson.Name.ShouldBe("halil");
        }
    }
}
