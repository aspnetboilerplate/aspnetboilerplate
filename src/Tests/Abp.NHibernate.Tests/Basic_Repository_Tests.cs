using Abp.Domain.Repositories;
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

        //[Fact] //Not working yet!
        public void Should_Get_All_People()
        {
            _personRepository.GetAllList().Count.ShouldBe(1);
        }
    }
}
